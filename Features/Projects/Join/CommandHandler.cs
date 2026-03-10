using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Application.Exceptions;
using OnSet.Domain.Enums;
using OnSet.Domain.Models;
using OnSet.Infrastructure.Data;
using OnSet.Infrastructure.Results;

namespace OnSet.Features.Projects.Join
{
    public class CommandHandler : IRequestHandler<Command, Result>
    {
        private readonly OnSetDbContext _context;

        public CommandHandler(OnSetDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return Result.Fail("User is not authenticated.");
            }

            var trimmedCode = request.JoinCode?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedCode))
            {
                return Result.Fail("Project code is required.");
            }

            var project = await _context.Projects
                .Include(p => p.UserProjects)
                .FirstOrDefaultAsync(p => p.ReferenceCode == trimmedCode, cancellationToken);

            if (project is null)
            {
                // Do not leak whether a project exists, keep message generic.
                return Result.Fail("Invalid project code.");
            }

            var alreadyMember = project.UserProjects.Any(up => up.UserId == request.UserId);
            if (alreadyMember)
            {
                return Result.Ok();
            }

            var userProject = UserProject.Create(request.UserId, ProjectRoles.PRODUCTION, project.Id);
            _context.UserProjects.Add(userProject);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
