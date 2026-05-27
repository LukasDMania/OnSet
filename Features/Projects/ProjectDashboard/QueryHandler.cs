using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet;
using OnSet.Application.Exceptions;
using OnSet.Application.Services;
using OnSet.Domain.Enums;
using OnSet.Infrastructure.Persistence;

namespace OnSet.Features.Projects.ProjectDashboard
{
    /// <summary>MediatR handler for this feature slice.</summary>
    public class QueryHandler : IRequestHandler<Query, Model>
    {
        private readonly OnSetDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectPermissionService _permissionService;

        public QueryHandler(
            OnSetDbContext context,
            ICurrentUserService currentUserService,
            IProjectPermissionService permissionService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionService = permissionService;
        }

        public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
        {
            if (request.Id is null)
            {
                throw new DomainRuleException("Project id is required.");
            }

            var project = await _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == request.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.ReferenceCode,
                    p.ProductionCompany,
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
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new
                {
                    d.Id,
                    Tag = d.Tags.FirstOrDefault(),
                    d.Metadata.FileName,
                    UploadedBy = d.User.FullName ?? d.User.UserName ?? string.Empty,
                    UploadedAt = d.CreatedAt,
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

            var capabilities = await _permissionService.GetDashboardCapabilitiesAsync(
                project.Id,
                _currentUserService.UserId,
                cancellationToken);

            return new Model
            {
                Id = project.Id,
                Name = project.Name,
                ReferenceCode = project.ReferenceCode,
                ProductionCompany = project.ProductionCompany,
                DocumentGroups = grouped,
                Capabilities = capabilities,
            };
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
