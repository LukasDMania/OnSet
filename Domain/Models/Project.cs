using System.ComponentModel.DataAnnotations.Schema;

namespace OnSet.Domain.Models
{
    public class Project : IEntity
    {
        [Column("ProjectID")]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

    }
}
