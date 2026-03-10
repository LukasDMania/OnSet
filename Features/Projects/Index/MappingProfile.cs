using AutoMapper;
using OnSet.Domain.Models;

namespace OnSet.Features.Projects.Index
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateProjection<Project, ProjectListItem>();
        }
    }
}
