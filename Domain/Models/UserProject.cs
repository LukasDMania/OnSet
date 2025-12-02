using OnSet.Domain.Enums;

namespace OnSet.Domain.Models
{
    public class UserProject
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; }

        public ProjectRoles RoleOnProject { get; set; }

        //nav
        public virtual User User { get; set; }
        public virtual Project Project { get; set; }

    }
}
