using OnSet.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Projects.Create
{
    public class InputModel
    {
        [Required]
        [Display(Name = "Project Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "My Role on Project")]
        public ProjectRoles CreatorRole { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ProjectStatus Status { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

    }
}
