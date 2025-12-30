using AutoMapper;
using OnSet.Domain.Models;

namespace OnSet.Features.Projects.Edit
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateProjection<Project, Command>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Location.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location.City))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Location.ProvinceOrState))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Location.Country))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.Location.ZipCode));
        }
    }
}
