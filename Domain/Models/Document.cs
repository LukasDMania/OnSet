using System.ComponentModel.DataAnnotations.Schema;

namespace OnSet.Domain.Models
{
    public class Document : IOnSetEntity
    {
        [Column("DocumentID")]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public string UserId { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }


        //nav
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }

        public virtual Contract Contract { get; set; }
    }
}
