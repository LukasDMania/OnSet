using AutoMapper;
using OnSet.Domain.Models;

namespace OnSet.Features.Projects.Index
{
    /// <summary>AutoMapper profile for this feature slice.</summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateProjection<Project, ProjectListItem>();
        }
    }
}
