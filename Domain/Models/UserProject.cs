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


        private UserProject() { }

        public static UserProject Create(string userId, ProjectRoles role, Project project)
        {
            return new UserProject
            {
                UserId = userId,
                RoleOnProject = role,
                Project = project
            };
        }

        //Overload for adding members to an EXISTING project
        public static UserProject Create(string userId, ProjectRoles role, int projectId)
        {
            return new UserProject
            {
                UserId = userId,
                RoleOnProject = role,
                ProjectId = projectId
            };
        }

        public void ChangeRoleOnProject(ProjectRoles newRole)
        {
            this.RoleOnProject = newRole;
        }
    }
}
