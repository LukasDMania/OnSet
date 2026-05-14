using MediatR;
using OnSet.Application.Notifications.Projects;
using OnSet.Domain.Models;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Data;
using OnSet.Infrastructure.Results;

namespace OnSet.Features.Projects.Create
{
    public class CommandHandler : IRequestHandler<Command, Result>
    {
        private readonly OnSetDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private OnSetDbContext context;
        private ICurrentUserService @object;

        public CommandHandler(OnSetDbContext context, ICurrentUserService @object)
        {
            this.context = context;
            this.@object = @object;
        }

        public CommandHandler(
            OnSetDbContext context,
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Fail("User is not authenticated.");
            }

            Address? location = null;

            var hasAnyAddressPart =
                !string.IsNullOrWhiteSpace(request.Street) ||
                !string.IsNullOrWhiteSpace(request.City) ||
                !string.IsNullOrWhiteSpace(request.ZipCode) ||
                !string.IsNullOrWhiteSpace(request.Country) ||
                !string.IsNullOrWhiteSpace(request.Province);

            if (hasAnyAddressPart)
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

            await _mediator.Publish(
                new ProjectCreatedNotification(project.Id, userId, project.Name),
                cancellationToken);

            return Result.Ok();
        }
    }
}
