using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Infrastructure.Data;
using OnSet.Application.Exceptions;

namespace OnSet.Features.Projects.Details
{
    /// <summary>MediatR handler for this feature slice.</summary>
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
            var query = _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == request.Id)
                .ProjectTo<Model>(_mapper.ConfigurationProvider);

            var model = await query.FirstOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                throw new NotFoundException("Project", request.Id!);
            }

            return model;
        }
    }
}
