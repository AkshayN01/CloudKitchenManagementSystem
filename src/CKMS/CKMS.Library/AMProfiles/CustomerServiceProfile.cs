using AutoMapper;
using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Contracts.DTOs.Customer.Response;

namespace CKMS.Library.AMProfiles
{
    public class CustomerServiceProfile : Profile
    {
        public CustomerServiceProfile() 
        {
            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId))
                .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(x => x.PhoneNumber))
                .ForMember(dest => dest.EmailId, src => src.MapFrom(x => x.EmailId))
                .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.UserName))
                .ForMember(dest => dest.LoyaltyPoints, src => src.MapFrom(x => x.LoyaltyPoints))
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Name))
                .ForMember(dest => dest.TotalOrder, src => src.MapFrom(x => x.TotalOrder));

            CreateMap<Address, AddressDTO>()
                .ForMember(dest => dest.AddressId, src => src.MapFrom(x => x.AddressId))
                .ForMember(dest => dest.AddressDetail, src => src.MapFrom(x => x.AddressDetail))
                .ForMember(dest => dest.PostalCode, src => src.MapFrom(x => x.PostalCode))
                .ForMember(dest => dest.Region, src => src.MapFrom(x => x.Region))
                .ForMember(dest => dest.City, src => src.MapFrom(x => x.City))
                .ForMember(dest => dest.Country, src => src.MapFrom(x => x.Country));
        }
    }
}
