using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Infrastructure.Data;

namespace OnSet.Features.Projects.Index
{
    /// <summary>MediatR handler for this feature slice.</summary>
    public class QueryHandler : IRequestHandler<Query, Model>
    {
        private readonly OnSetDbContext _db;
        private readonly IMapper _mapper;

        public QueryHandler(OnSetDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
        {
            // Only show projects the current user is a member of
            var projects = await _db.Projects
                .AsNoTracking()
                .Where(p => p.UserProjects.Any(up => up.UserId == request.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .ProjectTo<ProjectListItem>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new Model { Projects = projects };
        }
    }
}
