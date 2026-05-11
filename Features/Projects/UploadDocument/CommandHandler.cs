using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet;
using OnSet.Application.Exceptions;
using OnSet.Domain.Models;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Data;
using OnSet.Infrastructure.Results;

namespace OnSet.Features.Projects.UploadDocument
{
    public class CommandHandler : IRequestHandler<Command, Result>
    {
        private readonly OnSetDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;

        public CommandHandler(
            OnSetDbContext context,
            ICurrentUserService currentUserService,
            IStorageService storageService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _storageService = storageService;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Fail("User is not authenticated.");
            }

            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new NotFoundException("Project", request.ProjectId);
            }

            //nsure the current user is at least a member of the project.
            var isMember = await _context.UserProjects
                .AsNoTracking()
                .AnyAsync(up => up.ProjectId == request.ProjectId && up.UserId == userId, cancellationToken);

            if (!isMember)
            {
                return Result.Fail("You must be part of this project to upload documents.");
            }

            // For now, only the project creator (owner) can upload documents.
            // This is intentionally open for extension: in the future, i can
            // extend this to allow specific project roles or a configurable set
            // of uploaders managed by the project creator.
            if (!string.Equals(project.OwnerId, userId, StringComparison.Ordinal))
            {
                return Result.Fail("Only the project creator can upload documents at this time.");
            }

            if (request.File is null || request.File.Length <= 0)
            {
                return Result.Fail("A non-empty file is required.");
            }

            var storagePath = await _storageService.UploadFileAsync(request.File, $"projects/{request.ProjectId}");

            var metadata = new FileMetadata(
                fileName: request.File.FileName,
                sizeBytes: request.File.Length,
                mimeType: request.File.ContentType ?? "application/octet-stream");

            var document = new Document(
                projectId: request.ProjectId,
                userId: userId,
                metadata: metadata,
                filePath: storagePath,
                description: request.Description);

            document.Tags.Add(request.Tag);

            _context.Documents.Add(document);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}

