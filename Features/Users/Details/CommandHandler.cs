using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using OnSet.Domain.Models;
using OnSet.Features.Projects.Details;

namespace OnSet.Features.Users.Details
{
    public class CommandHandler : IRequestHandler<Query, Model> 
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public CommandHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                throw new ArgumentException("User ID cannot be null");
            }

            var user = _userManager.Users
                .Include(u => u.UserProjects)
                    .ThenInclude(up => up.Project)
                .Include(u => u.Contracts)
                    .ThenInclude(c => c.Document)
                .Where(u => u.Id == request.Id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.Id} not found."); 
            }

            var model = await user
                .ProjectTo<Model>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (model == null) 
            {
                throw new Exception("null model after mapping");
            }

            return model;
        }
    }
}
