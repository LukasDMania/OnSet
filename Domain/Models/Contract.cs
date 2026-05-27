using OnSet.Domain.Enums;
using OnSet.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnSet.Domain.Models
{
    /// <summary>Domain model or value object.</summary>
    public class Contract : IOnSetEntity, IAuditableEntity
    {
        [Key]
        [Column("ContractID")]
        public int Id { get; private set; }

        [Required]
        public int DocumentId { get; private set; }

        [Required]
        public string UserId { get; private set; }

        [Required]
        public ContractStatus Status { get; private set; } = ContractStatus.PENDING;

        public SignatureInfo? Signature { get; private set; }

        [StringLength(200)]
        public string? Notes { get; private set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        // Nav
        public virtual Document Document { get; private set; }
        public virtual User User { get; private set; }

        private Contract() { }

        public Contract(int documentId, string userId)
        {
            DocumentId = documentId;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void Sign(string userId)
        {
            Status = ContractStatus.SIGNED;
            Signature = new SignatureInfo(userId, DateTime.UtcNow);
        }

        public void Decline(string note)
        {
            Status = ContractStatus.DECLINED;
            Notes = note;
        }
    }
}
