using AutoMapper;
using OnSet.Domain.Models;
using OnSet.Features.Users.Details;

namespace OnSet.Features.Projects.Details
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateProjection<Project, Model>();
        }
    }
}
