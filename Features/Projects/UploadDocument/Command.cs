using MediatR;
using Microsoft.AspNetCore.Http;
using OnSet.Domain.Enums;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Projects.UploadDocument
{
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

