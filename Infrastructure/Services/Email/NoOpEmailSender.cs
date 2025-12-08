using Microsoft.AspNetCore.Identity.UI.Services;

namespace OnSet.Infrastructure.Services.Email
{
    public class NoOpEmailSender : IEmailSender
    {
        private readonly ILogger<NoOpEmailSender> _logger;

        public NoOpEmailSender(ILogger<NoOpEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation(
                "NoOpEmailSender: Email Sent to {Email}\nSubject: {Subject}\nBody: {Body}",
                email, subject, htmlMessage
            );

            return Task.CompletedTask;
        }
    }
}
