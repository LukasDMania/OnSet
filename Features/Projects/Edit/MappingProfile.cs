using AutoMapper;
using OnSet.Domain.Models;

namespace OnSet.Features.Projects.Edit
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateProjection<Project, Command>();
        }
    }
}
