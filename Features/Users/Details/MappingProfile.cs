using OnSet.Domain.Models;
using AutoMapper;
using OnSet.Domain.Enums;

namespace OnSet.Features.Users.Details
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateProjection<User, Model>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Value))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Value))
                .ForMember(dest => dest.MainOccupationRole, opt => opt.MapFrom(src => src.MainOccupationRole.ToString()))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.HomeAddress.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.HomeAddress.City))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.HomeAddress.ProvinceOrState))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.HomeAddress.ZipCode))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.HomeAddress.Country))
                .ForMember(dest => dest.SpokenLanguages, opt => opt.MapFrom(src => src.SpokenLanguages.Select(l => l.ToString())))
                .ForMember(dest => dest.EmergencyContactName, opt => opt.MapFrom(src => src.EmergencyContact.Name))
                .ForMember(dest => dest.EmergencyContactPhone, opt => opt.MapFrom(src => src.EmergencyContact.Phone))
                .ForMember(dest => dest.Projects, opt => opt.MapFrom(src => src.UserProjects))
                .ForMember(dest => dest.Contracts, opt => opt.MapFrom(src => src.Contracts));

            CreateProjection<UserProject, UserProjectDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.RoleOnProject, opt => opt.MapFrom(src => (ProjectRoles)src.RoleOnProject));

            CreateProjection<Contract, ContractDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DocumentPath, opt => opt.MapFrom(src => src.Document.FilePath))
                .ForMember(dest => dest.DocumentDescription, opt => opt.MapFrom(src => src.Document.Description))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ContractStatus)src.Status))
                .ForMember(dest => dest.SignedAt, opt => opt.MapFrom(src => src.Signature != null ? (DateTime?)src.Signature.SignedAt : null))
                .ForMember(dest => dest.SignedByUserId, opt => opt.MapFrom(src => src.Signature != null ? src.Signature.SignedByUserId : null));
        }
    }
}
