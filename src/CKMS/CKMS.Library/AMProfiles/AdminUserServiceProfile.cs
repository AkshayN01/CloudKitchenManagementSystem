using AutoMapper;
using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DTOs.AdminUser.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.AMProfiles
{
    public class AdminUserServiceProfile : Profile
    {
        public AdminUserServiceProfile() 
        {
            CreateMap<Kitchen, KitchenDTO>()
                .ForMember(dest => dest.KitchenId, src => src.MapFrom(x => x.KitchenId))
                .ForMember(dest => dest.KitchenName, src => src.MapFrom(x => x.KitchenName))
                .ForMember(dest => dest.Address, src => src.MapFrom(x => x.Address))
                .ForMember(dest => dest.PostalCode, src => src.MapFrom(x => x.PostalCode))
                .ForMember(dest => dest.City, src => src.MapFrom(x => x.City))
                .ForMember(dest => dest.Region, src => src.MapFrom(x => x.Region))
                .ForMember(dest => dest.Country, src => src.MapFrom(x => x.Country));

            CreateMap<AdminUser, AdminUserDTO>()
                .ForMember(dest => dest.UserId, src => src.MapFrom(x => x.UserId))
                .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.UserName))
                .ForMember(dest => dest.EmailId, src => src.MapFrom(x => x.EmailId))
                .ForMember(dest => dest.RoleId, src => src.MapFrom(x => x.RoleId))
                .ForMember(dest => dest.FullName, src => src.MapFrom(x => x.FullName))
                .ForMember(dest => dest.KitchenId, src => src.MapFrom(x => x.KitchenId));
        }
    }
}
