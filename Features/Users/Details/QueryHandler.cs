using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnSet.Domain.Models;
using OnSet.Application.Exceptions;

namespace OnSet.Features.Users.Details
{
    /// <summary>MediatR handler for this feature slice.</summary>
    public class QueryHandler : IRequestHandler<Query, Model> 
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public QueryHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                throw new DomainRuleException("User ID cannot be null.");
            }

            var model = await _userManager.Users
                .Where(u => u.Id == request.Id)
                .ProjectTo<Model>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                throw new NotFoundException("User", request.Id!);
            }


            return model;
        }
    }
}
