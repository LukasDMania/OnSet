using MediatR;
using OnSet.Application.Exceptions;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Data;

namespace OnSet.Features.Projects.Edit
{
    /// <summary>MediatR handler for this feature slice.</summary>
    public class CommandHandler : IRequestHandler<Command, Unit>
    {
        private readonly OnSetDbContext _db;

        public CommandHandler(OnSetDbContext db)
        {
            _db = db;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = await _db.Projects.FindAsync([request.Id], cancellationToken);
            if (project == null)
            {
                throw new NotFoundException("Project", request.Id);
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
                    street: request.Street!,
                    city: request.City!,
                    provinceOrState: request.Province,
                    country: request.Country!,
                    zipCode: request.ZipCode!
                );
            }

            project.UpdateDetails(
                name: request.Name,
                description: request.Description,
                startDate: request.StartDate,
                endDate: request.EndDate,
                status: request.Status,
                clientName: request.ClientName,
                referenceCode: request.ReferenceCode,
                budget: request.Budget,
                location: location
            );

            await _db.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
