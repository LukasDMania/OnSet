using AutoMapper;
using OnSet.Domain.Models;

namespace OnSet.Features.Projects.Details
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateProjection<Project, Model>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Location != null ? src.Location.Street : null))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location != null ? src.Location.City : null))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Location != null ? src.Location.ProvinceOrState : null))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Location != null ? src.Location.Country : null))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.Location != null ? src.Location.ZipCode : null))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.UserName : null));
        }
    }
}
