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
            return default;
        }
    }
}
