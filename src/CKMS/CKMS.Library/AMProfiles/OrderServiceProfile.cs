using AutoMapper;
using CKMS.Contracts.DBModels.OrderService;
using CKMS.Contracts.DTOs.Order.Response;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.AMProfiles
{
    public class OrderServiceProfile : Profile
    {
        public OrderServiceProfile() 
        {
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderId, src => src.MapFrom(x => x.OrderId))
                .ForMember(dest => dest.OrderDate, src => src.MapFrom(x => x.OrderDate))
                .ForMember(dest => dest.InProgressDate, src => src.MapFrom(x => x.InProgressTime))
                .ForMember(dest => dest.OutForDeliveryDate, src => src.MapFrom(x => x.OutForDeliveryTime))
                .ForMember(dest => dest.DeliveredDate, src => src.MapFrom(x => x.DeliveryTime))
                .ForMember(dest => dest.Address, src => src.MapFrom(x => x.Address))
                .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId))
                .ForMember(dest => dest.GrossAmount, src => src.MapFrom(x => x.GrossAmount))
                .ForMember(dest => dest.NetAmount, src => src.MapFrom(x => x.NetAmount))
                .ForMember(dest => dest.Status, src => src.MapFrom(x =>
                Library.Generic.Utility.GetEnumStringValue<OrderStatus>(x.Status)));


            CreateMap<Order, OrderDetailDTO>()
                .ForMember(dest => dest.OrderId, src => src.MapFrom(x => x.OrderId))
                .ForMember(dest => dest.OrderDate, src => src.MapFrom(x => x.OrderDate))
                .ForMember(dest => dest.InProgressDate, src => src.MapFrom(x => x.InProgressTime))
                .ForMember(dest => dest.OutForDeliveryDate, src => src.MapFrom(x => x.OutForDeliveryTime))
                .ForMember(dest => dest.DeliveredDate, src => src.MapFrom(x => x.DeliveryTime))
                .ForMember(dest => dest.Address, src => src.MapFrom(x => x.Address))
                .ForMember(dest => dest.GrossAmount, src => src.MapFrom(x => x.GrossAmount))
                .ForMember(dest => dest.NetAmount, src => src.MapFrom(x => x.NetAmount))
                .ForMember(dest => dest.Status, src => src.MapFrom(x => 
                Library.Generic.Utility.GetEnumStringValue<OrderStatus>(x.Status)));

            CreateMap<Order, OrderCartDTO>()
                .ForMember(dest => dest.OrderId, src => src.MapFrom(x => x.OrderId))
                .ForMember(dest => dest.OrderDate, src => src.MapFrom(x => x.OrderDate))
                .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId))
                .ForMember(dest => dest.GrossAmount, src => src.MapFrom(x => x.GrossAmount))
                .ForMember(dest => dest.NetAmount, src => src.MapFrom(x => x.NetAmount));
        }
    }
}
