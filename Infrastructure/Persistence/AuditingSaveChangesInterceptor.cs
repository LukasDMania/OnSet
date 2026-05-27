using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using OnSet.Application.Services;
using OnSet.Domain.Models;
using OnSet.Infrastructure.Persistence;

namespace OnSet.Infrastructure.Persistence;

/// <summary>
/// Stamps <see cref="IAuditableEntity"/> timestamps and appends <see cref="EntityChangeAudit"/> rows for the same save.
/// </summary>
public class AuditingSaveChangesInterceptor : SaveChangesInterceptor
{
    public const string OperationInsert = "Insert";
    public const string OperationUpdate = "Update";
    public const string OperationDelete = "Delete";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly HashSet<Type> IgnoredClrTypes =
    [
        typeof(CommandAuditLog),
        typeof(EntityChangeAudit)
    ];

    private static readonly Action<ILogger, string, Exception?> EntityAuditWriteFailed =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(2, "EntityChangeAuditWriteFailed"),
            "Building entity change audit failed for {EntityName}; change is still saved.");

    private readonly ICurrentUserService _currentUser;
    private readonly ILogger _logger;

    public AuditingSaveChangesInterceptor(ICurrentUserService currentUser, ILoggerFactory loggerFactory)
    {
        _currentUser = currentUser;
        _logger = loggerFactory.CreateLogger("OnSet.EntityAudit");
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is OnSetDbContext db)
            ApplyAuditing(db);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditing(OnSetDbContext context)
    {
        var utc = DateTime.UtcNow;
        var userId = string.IsNullOrWhiteSpace(_currentUser.UserId) ? null : _currentUser.UserId;
        var correlationId = _currentUser.CorrelationId;
        var rows = new List<EntityChangeAudit>();

        foreach (var entry in context.ChangeTracker.Entries().ToList())
        {
            var clrType = entry.Metadata.ClrType;
            if (IgnoredClrTypes.Contains(clrType))
                continue;

            if (entry.Entity is IAuditableEntity auditable)
                ApplyTimestamps(entry, auditable, utc);

            if (entry.Entity is not IAuditableEntity)
                continue;

            if (entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            try
            {
                var row = BuildAuditRow(entry, userId, correlationId, utc);
                if (row is not null)
                    rows.Add(row);
            }
            catch (Exception ex)
            {
                EntityAuditWriteFailed(_logger, entry.Metadata.DisplayName(), ex);
            }
        }

        if (rows.Count > 0)
            context.EntityChangeAudits.AddRange(rows);
    }

    private static void ApplyTimestamps(EntityEntry entry, IAuditableEntity auditable, DateTime utc)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                auditable.CreatedAt = utc;
                auditable.UpdatedAt = null;
                break;
            case EntityState.Modified:
                auditable.UpdatedAt = utc;
                break;
        }
    }

    private static EntityChangeAudit? BuildAuditRow(
        EntityEntry entry,
        string? userId,
        string? correlationId,
        DateTime utc)
    {
        var operation = entry.State switch
        {
            EntityState.Added => OperationInsert,
            EntityState.Modified => OperationUpdate,
            EntityState.Deleted => OperationDelete,
            _ => null
        };

        if (operation is null)
            return null;

        var entityName = entry.Metadata.ClrType.Name;
        var entityId = BuildEntityId(entry);

        string? oldJson = null;
        string? newJson = null;

        switch (entry.State)
        {
            case EntityState.Added:
                newJson = SerializeProperties(entry, useOriginal: false);
                break;
            case EntityState.Deleted:
                oldJson = SerializeProperties(entry, useOriginal: true);
                break;
            case EntityState.Modified:
                (oldJson, newJson) = BuildModifiedSnapshots(entry);
                if (oldJson is null && newJson is null)
                    return null;
                break;
        }

        return new EntityChangeAudit
        {
            EntityName = entityName,
            EntityId = entityId,
            OperationType = operation,
            OldValuesJson = oldJson,
            NewValuesJson = newJson,
            ChangedByUserId = userId,
            CorrelationId = correlationId,
            ChangedAt = utc
        };
    }

    private static string BuildEntityId(EntityEntry entry)
    {
        var parts = entry.Properties
            .Where(p => p.Metadata.IsPrimaryKey())
            .Select(p => (p.OriginalValue ?? p.CurrentValue)?.ToString() ?? string.Empty);
        return string.Join("|", parts);
    }

    private static string? SerializeProperties(EntityEntry entry, bool useOriginal)
    {
        var dict = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var prop in entry.Properties.Where(p => !p.Metadata.IsPrimaryKey()))
        {
            if (prop.Metadata.IsShadowProperty())
                continue;
            var value = useOriginal ? prop.OriginalValue : prop.CurrentValue;
            dict[prop.Metadata.Name] = Sanitize(value);
        }

        return SerializeDictionary(dict);
    }

    private static (string? OldJson, string? NewJson) BuildModifiedSnapshots(EntityEntry entry)
    {
        var oldDict = new Dictionary<string, object?>(StringComparer.Ordinal);
        var newDict = new Dictionary<string, object?>(StringComparer.Ordinal);

        foreach (var prop in entry.Properties.Where(p => p.IsModified && !p.Metadata.IsPrimaryKey()))
        {
            if (prop.Metadata.IsShadowProperty())
                continue;
            oldDict[prop.Metadata.Name] = Sanitize(prop.OriginalValue);
            newDict[prop.Metadata.Name] = Sanitize(prop.CurrentValue);
        }

        return (SerializeDictionary(oldDict), SerializeDictionary(newDict));
    }

    private static object? Sanitize(object? value)
    {
        if (value is null)
            return null;
        if (value is byte[])
            return "[binary]";
        if (value is string or bool or DateTime or DateTimeOffset or Guid or Enum)
            return value;
        if (value.GetType().IsPrimitive || value is decimal)
            return value;
        if (value is IEnumerable seq and not string)
        {
            var list = new List<object?>();
            foreach (var item in seq)
                list.Add(Sanitize(item));
            return list;
        }

        try
        {
            return JsonSerializer.Serialize(value, JsonOptions);
        }
        catch
        {
            return value.ToString();
        }
    }

    private static string? SerializeDictionary(Dictionary<string, object?> dict)
    {
        if (dict.Count == 0)
            return null;
        try
        {
            return JsonSerializer.Serialize(dict, JsonOptions);
        }
        catch
        {
            return null;
        }
    }
}
