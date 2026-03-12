using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet;
using OnSet.Application.Exceptions;
using OnSet.Domain.Enums;
using OnSet.Infrastructure.Data;

namespace OnSet.Features.Projects.ProjectDashboard
{
    public class QueryHandler : IRequestHandler<Query, Model>
    {
        private readonly OnSetDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public QueryHandler(OnSetDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
        {
            if (request.Id is null)
            {
                throw new DomainRuleException("Project id is required.");
            }

            var userId = _currentUserService.UserId;

            // Enforce that only members (or owner) can see the dashboard
            var isMember = await _context.UserProjects
                .AsNoTracking()
                .AnyAsync(up => up.ProjectId == request.Id && up.UserId == userId, cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenAccessException("You must be part of this project to view its dashboard.");
            }

            var project = await _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == request.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.ReferenceCode,
                    p.ClientName,
                    p.OwnerId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (project is null)
            {
                throw new NotFoundException("Project", request.Id.Value);
            }

            var documents = await _context.Documents
                .AsNoTracking()
                .Include(d => d.User)
                .Where(d => d.ProjectId == project.Id && !d.IsArchived)
                .OrderByDescending(d => d.UploadedAt)
                .Select(d => new
                {
                    d.Id,
                    Tag = d.Tags.FirstOrDefault(),
                    d.Metadata.FileName,
                    UploadedBy = d.User.FullName ?? d.User.UserName ?? string.Empty,
                    d.UploadedAt,
                    d.FilePath
                })
                .ToListAsync(cancellationToken);

            var grouped = documents
                .GroupBy(d => d.Tag)
                .OrderBy(g => g.Key.ToString())
                .Select(g => new DocumentGroup
                {
                    Tag = g.Key,
                    DisplayName = GetDisplayNameForTag(g.Key),
                    Documents = g
                        .Select(d => new DocumentDto
                        {
                            Id = d.Id,
                            FileName = d.FileName,
                            UploadedBy = d.UploadedBy,
                            UploadedAt = d.UploadedAt,
                            FilePath = d.FilePath
                        })
                        .ToList()
                })
                .ToList();

            var model = new Model
            {
                Id = project.Id,
                Name = project.Name,
                ReferenceCode = project.ReferenceCode,
                ClientName = project.ClientName,
                DocumentGroups = grouped,
                // Currently only the project creator can upload documents.
                // This is intentionally open for extension: in the future we can
                // extend this to allow specific project roles or a configurable
                // list of uploaders defined by the project creator.
                CanUploadDocuments = string.Equals(project.OwnerId, userId, StringComparison.Ordinal)
            };

            return model;
        }

        private static string GetDisplayNameForTag(DocumentTags tag) =>
            tag switch
            {
                DocumentTags.CALLSHEET => "Call Sheets",
                DocumentTags.SCENARIO => "Scenarios",
                DocumentTags.SCHEDULE => "Schedules",
                DocumentTags.LOGISTICS => "Logistics",
                DocumentTags.OTHER => "Other Documents",
                _ => tag.ToString()
            };
    }
}

