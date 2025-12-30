using MediatR;
using OnSet.Domain.Models;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Data;
using System.Security.Claims;

namespace OnSet.Features.Projects.Create
{
    public class CommandHandler : IRequestHandler<Command, CommandResult>
    {
        private readonly OnSetDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommandHandler(
            OnSetDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
        }

        public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new CommandResult
                {
                    Success = false,
                    Errors = new[] { "User is not authenticated." }
                };
            }

            Address? location = null;

            if (!string.IsNullOrWhiteSpace(request.Street))
            {
                location = new Address(
                    request.Street!,
                    request.City!,
                    request.Province,
                    request.Country!,
                    request.ZipCode!
                );
            }

            var project = Project.Create(
                name: request.ProjectName,
                startDate: request.StartDate,
                status: request.Status,
                creatorRole: request.CreatorRole,
                ownerId: userId,
                description: request.Description,
                clientName: request.ClientName,
                referenceCode: request.ReferenceCode,
                budget: request.Budget,
                location: location
            );

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);

            return new CommandResult { Success = true };
        }
    }
}
