using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Infrastructure.Data;

namespace OnSet.Features.Projects.Edit
{
    public class QueryHandler : IRequestHandler<Query, Command>
    {
        private readonly OnSetDbContext _db;
        private readonly AutoMapper.IConfigurationProvider _configurationProvider;

        public QueryHandler(OnSetDbContext db, AutoMapper.IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public Task<Command?> Handle(Query message, CancellationToken cancellationToken)
        {
            //how to handle?
            return _db.Projects
                .Where(p => p.Id == message.Id)
                .ProjectTo<Command>(_configurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
