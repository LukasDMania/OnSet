using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using OnSet.Application.Notifications.Projects;
using OnSet.Domain.Models;

namespace OnSet.Infrastructure.Notifications.Projects;

public class ProjectCreatedNotificationHandler : INotificationHandler<ProjectCreatedNotification>
{
    private readonly IEmailSender _emailSender;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ProjectCreatedNotificationHandler> _logger;

    public ProjectCreatedNotificationHandler(
        IEmailSender emailSender,
        UserManager<User> userManager,
        ILogger<ProjectCreatedNotificationHandler> logger)
    {
        _emailSender = emailSender;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task Handle(ProjectCreatedNotification notification, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(notification.OwnerUserId);
        if (string.IsNullOrWhiteSpace(user?.Email))
        {
            _logger.LogWarning(
                "ProjectCreated: no email for owner {OwnerUserId}, project {ProjectId}",
                notification.OwnerUserId,
                notification.ProjectId);
            return;
        }

        var subject = $"Project created: {notification.ProjectName}";
        var body =
            $"<p>Your project <strong>{System.Net.WebUtility.HtmlEncode(notification.ProjectName)}</strong> " +
            $"was created successfully.</p><p>Project id: {notification.ProjectId}</p>";

        await _emailSender.SendEmailAsync(user.Email, subject, body);
    }
}
