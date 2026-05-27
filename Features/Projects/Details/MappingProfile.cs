using AutoMapper;
using OnSet.Domain.Models;

namespace OnSet.Features.Projects.Details
{
    /// <summary>AutoMapper profile for this feature slice.</summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateProjection<Project, Model>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.ProductionCompanyLocation != null ? src.ProductionCompanyLocation.Street : null))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.ProductionCompanyLocation != null ? src.ProductionCompanyLocation.City : null))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.ProductionCompanyLocation != null ? src.ProductionCompanyLocation.ProvinceOrState : null))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.ProductionCompanyLocation != null ? src.ProductionCompanyLocation.Country : null))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ProductionCompanyLocation != null ? src.ProductionCompanyLocation.ZipCode : null))
                .ForMember(dest => dest.InvoiceCompanyName, opt => opt.MapFrom(src => src.InvoiceCompanyName))
                .ForMember(dest => dest.InvoiceStreet, opt => opt.MapFrom(src => src.InvoiceAddress != null ? src.InvoiceAddress.Street : null))
                .ForMember(dest => dest.InvoiceCity, opt => opt.MapFrom(src => src.InvoiceAddress != null ? src.InvoiceAddress.City : null))
                .ForMember(dest => dest.InvoiceProvince, opt => opt.MapFrom(src => src.InvoiceAddress != null ? src.InvoiceAddress.ProvinceOrState : null))
                .ForMember(dest => dest.InvoiceCountry, opt => opt.MapFrom(src => src.InvoiceAddress != null ? src.InvoiceAddress.Country : null))
                .ForMember(dest => dest.InvoiceZipCode, opt => opt.MapFrom(src => src.InvoiceAddress != null ? src.InvoiceAddress.ZipCode : null))
                .ForMember(dest => dest.InvoiceVatNumber, opt => opt.MapFrom(src => src.InvoiceVatNumber))
                .ForMember(dest => dest.InvoiceReference, opt => opt.MapFrom(src => src.InvoiceReference))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.UserName : null));
        }
    }
}
