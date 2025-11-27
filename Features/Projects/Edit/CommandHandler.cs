using MediatR;
using OnSet.Infrastructure.Data;

namespace OnSet.Features.Projects.Edit
{
    public class CommandHandler : IRequestHandler<Command, Unit>
    {
        private readonly OnSetDbContext _db;

        public CommandHandler(OnSetDbContext db) => _db = db;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = await _db.Projects.FindAsync(request.Id);

            project.Name = request.Name;
            project.Description = request.Description;

            await _db.SaveChangesAsync(cancellationToken);

            return default;
        }
    }
}
