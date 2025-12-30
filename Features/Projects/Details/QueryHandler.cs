using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Infrastructure.Data;

namespace OnSet.Features.Projects.Details
{
    public class QueryHandler : IRequestHandler<Query, Model>
    {
        private readonly OnSetDbContext _context;
        private readonly IMapper _mapper;

        public QueryHandler(OnSetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
        {
            var model = await _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == request.Id)
                .ProjectTo<Model>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                throw new KeyNotFoundException($"Project with ID {request.Id} not found.");
            }

            return model;
        }


    }
}
