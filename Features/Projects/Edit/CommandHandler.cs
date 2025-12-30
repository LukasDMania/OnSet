using MediatR;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Data;

namespace OnSet.Features.Projects.Edit
{
    public class CommandHandler : IRequestHandler<Command, Unit>
    {
        private readonly OnSetDbContext _db;
        public CommandHandler(OnSetDbContext db)
        {
            _db = db;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = await _db.Projects.FindAsync(request.Id);

            if (project == null) throw new KeyNotFoundException($"Project {request.Id} not found.");

            var location = new Address(
                street: request.Street ?? string.Empty,
                city: request.City ?? string.Empty,
                provinceOrState: request.Province ?? string.Empty,
                country: request.Country ?? string.Empty,
                zipCode: request.ZipCode ?? string.Empty
            );

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
