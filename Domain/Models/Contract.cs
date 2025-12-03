using System.ComponentModel.DataAnnotations.Schema;

namespace OnSet.Domain.Models
{
    public class Contract : IOnSetEntity
    {
        [Column("ContractID")]
        public int Id { get; set; }

        public int DocumentId { get; set; }
        public string UserId { get; set; }

        public bool IsSigned { get; set; }
        public DateTime? SignedAt { get; set; }

        //nav
        public virtual Document Document { get; set; }
        public virtual User User { get; set; }
    }
}
