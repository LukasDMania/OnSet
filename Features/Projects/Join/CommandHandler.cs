using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Application.Exceptions;
using OnSet.Domain.Enums;
using OnSet.Domain.Models;
using OnSet.Infrastructure.Persistence;
using OnSet.Infrastructure.Results;

namespace OnSet.Features.Projects.Join
{
    /// <summary>MediatR handler for this feature slice.</summary>
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

            // Defensive: NameIdentifier should be AspNetUsers.Id, but if claim mapping changes (or older cookies exist),
            // this prevents a hard FK crash and makes the failure actionable.
            var resolvedUserId = request.UserId.Trim();
            var userExists = await _context.Users.AnyAsync(u => u.Id == resolvedUserId, cancellationToken);
            if (!userExists)
            {
                // Fallback: sometimes the "id" we get is email/username.
                var byEmailOrUserName = await _context.Users
                    .Where(u => u.Email == resolvedUserId || u.UserName == resolvedUserId)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(byEmailOrUserName))
                {
                    resolvedUserId = byEmailOrUserName;
                }
                else
                {
                    return Result.Fail("Your user account could not be resolved. Please sign out and sign in again.");
                }
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
                //o not leak whether a project exists, keep message generic.
                return Result.Fail("Invalid project code.");
            }

            var alreadyMember = project.UserProjects.Any(up => up.UserId == resolvedUserId);
            if (alreadyMember)
            {
                return Result.Ok();
            }

            var userProject = UserProject.Create(resolvedUserId, ProjectRoles.UNASSIGNED, project.Id);
            _context.UserProjects.Add(userProject);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
