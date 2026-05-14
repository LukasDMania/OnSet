using OnSet.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Domain.Models
{
    public class UserProject : IAuditableEntity
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; }

        public ProjectRoles RoleOnProject { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        //nav
        public virtual User User { get; set; }
        public virtual Project Project { get; set; }


        private UserProject() { }

        public static UserProject Create(string userId, ProjectRoles role, Project project)
        {
            return new UserProject
            {
                UserId = userId,
                RoleOnProject = role,
                Project = project,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
        }

        //Overload for adding members to an EXISTING project
        public static UserProject Create(string userId, ProjectRoles role, int projectId)
        {
            return new UserProject
            {
                UserId = userId,
                RoleOnProject = role,
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
        }

        public void ChangeRoleOnProject(ProjectRoles newRole)
        {
            this.RoleOnProject = newRole;
        }
    }
}
