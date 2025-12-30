using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Infrastructure.Data;

namespace OnSet.Features.Projects.Edit
{
    public class QueryHandler : IRequestHandler<Query, Command>
    {
        private readonly OnSetDbContext _db;
        private readonly IMapper _mapper;

        public QueryHandler(OnSetDbContext db, IMapper configuration)
        {
            _db = db;
            _mapper = configuration;
        }

        public async Task<Command> Handle(Query message, CancellationToken token)
        {
            var model = await _db
                .Projects
                .Where(d => d.Id == message.Id)
                .ProjectTo<Command>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(token);

            if (model == null)
            {
                throw new KeyNotFoundException($"Project with ID {message.Id} not found.");
            }

            return model;
        }
    }
}
