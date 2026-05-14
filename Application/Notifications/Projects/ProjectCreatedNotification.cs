using MediatR;

namespace OnSet.Application.Notifications.Projects;

/// <summary>
/// Raised after a project is persisted. Subscribe with <see cref="INotificationHandler{TNotification}"/> for side effects (email, analytics, etc.).
/// </summary>
public record ProjectCreatedNotification(int ProjectId, string OwnerUserId, string ProjectName) : INotification;
