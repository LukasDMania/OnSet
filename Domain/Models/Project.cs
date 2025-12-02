using System.ComponentModel.DataAnnotations.Schema;

namespace OnSet.Domain.Models
{
    public class Project : IOnSetEntity
    {
        [Column("ProjectID")]
        public int Id { get; set; }


        public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    }
}
