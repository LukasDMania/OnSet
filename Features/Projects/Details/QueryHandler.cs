using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Infrastructure.Data;
using OnSet.Application.Exceptions;
using OnSet;

namespace OnSet.Features.Projects.Details
{
    /// <summary>MediatR handler for this feature slice.</summary>
    public class QueryHandler : IRequestHandler<Query, Model>
    {
        private readonly OnSetDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public QueryHandler(OnSetDbContext context, IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var query = _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == request.Id)
                .ProjectTo<Model>(_mapper.ConfigurationProvider);

            // Enforce that only members (or owner) can see full project details
            var isMember = await _context.UserProjects
                .AnyAsync(up => up.ProjectId == request.Id && up.UserId == userId, cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenAccessException("You must be part of this project to view its details.");
            }

            var model = await query.FirstOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                throw new NotFoundException("Project", request.Id!);
            }

            return model;
        }


    }
}
