using OnSet.Domain.Enums;
using OnSet.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnSet.Domain.Models
{
    public class Document : IOnSetEntity, IAuditableEntity
    {
        [Key]
        [Column("DocumentID")]
        public int Id { get; private set; }

        [Required]
        public int ProjectId { get; private set; }

        [Required]
        public string UserId { get; private set; }


        [Required]
        public FileMetadata Metadata { get; private set; }

        [Required]
        [StringLength(300)]
        public string FilePath { get; private set; }

        [DataType(DataType.DateTime)]
        [Column("UploadedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [StringLength(200)]
        public string? Description { get; private set; }

        public List<DocumentTags> Tags { get; private set; } = new List<DocumentTags>();

        public bool IsArchived { get; private set; } = false;

        // Navigation
        public virtual Project Project { get; private set; }
        public virtual User User { get; private set; }

        public virtual Contract? Contract { get; private set; }


        private Document() { }

        public Document(int projectId, string userId, FileMetadata metadata, string filePath, string? description = null)
        {
            ProjectId = projectId;
            UserId = userId;
            Metadata = metadata;
            FilePath = filePath;
            Description = description;
        }
    }
}
