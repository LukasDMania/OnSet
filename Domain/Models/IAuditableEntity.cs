namespace OnSet.Domain.Models;

/// <summary>
/// Aggregate / entity rows that receive automatic <see cref="CreatedAt"/> / <see cref="UpdatedAt"/>
/// stamps at save time and optional change-history persistence.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
