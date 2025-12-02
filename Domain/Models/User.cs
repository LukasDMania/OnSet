using Microsoft.AspNetCore.Identity;

namespace OnSet.Domain.Models
{
    public class User : IdentityUser
    {


        //nav
        public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();

        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}