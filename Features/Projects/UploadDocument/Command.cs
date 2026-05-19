using MediatR;
using Microsoft.AspNetCore.Http;
using OnSet.Domain.Enums;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Projects.UploadDocument
{
    /// <summary>Uploads a tagged document to a project.</summary>
    /// <remarks>POST <c>/Projects/{id}?handler=Upload</c>; validated by <see cref="Validator"/>.</remarks>
    public record Command : IRequest<Result>
    {
        [Required]
        public int ProjectId { get; init; }

        [Required]
        public DocumentTags Tag { get; init; }

        [StringLength(200)]
        public string? Description { get; init; }

        [Required]
        public IFormFile? File { get; init; }
    }
}

