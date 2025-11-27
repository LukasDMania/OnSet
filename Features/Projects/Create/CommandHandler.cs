using MediatR;
using OnSet.Domain.Models;
using OnSet.Infrastructure.Data;

namespace OnSet.Features.Projects.Create
{
    public class CommandHandler : IRequestHandler<Command, int>
    {
        private readonly OnSetDbContext _db;

        public CommandHandler(OnSetDbContext db)
        {
            _db = db;
        }

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
            };

            await _db.Projects.AddAsync(project, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            return project.Id;
        }
    }
}
