using AutoMapper;
using OnSet.Domain.Enums;
using OnSet.Domain.Models;

namespace OnSet.Features.Users.OtherUserDetails
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateProjection<User, Model>()
            .ForMember(d => d.FirstName,
                o => o.MapFrom(s => s.FirstName.Value))
            .ForMember(d => d.LastName,
                o => o.MapFrom(s => s.LastName.Value))
            .ForMember(d => d.MainOccupationRole,
                o => o.MapFrom(s => s.MainOccupationRole))
            .ForMember(d => d.SpokenLanguages,
                o => o.MapFrom(s => s.SpokenLanguages ?? new List<Languages>()));
        }
    }
}
