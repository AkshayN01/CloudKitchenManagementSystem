using AutoMapper;
using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Contracts.DTOs.Inventory.Response;

namespace CKMS.Library.AMProfiles
{
    public class InventoryServiceProfile : Profile
    {
        public InventoryServiceProfile() 
        {
            CreateMap<Inventory, InventoryDTO>()
                .ForMember(dest => dest.InventoryId, src => src.MapFrom(x => x.InventoryId))
                .ForMember(dest => dest.Quantity, src => src.MapFrom(x => x.Quantity))
                .ForMember(dest => dest.RestockThreshold, src => src.MapFrom(x => x.RestockThreshold))
                .ForMember(dest => dest.MaxStockLevel, src => src.MapFrom(x => x.MaxStockLevel))
                .ForMember(dest => dest.InventoryName, src => src.MapFrom(x => x.InventoryName))
                .ForMember(dest => dest.KitchenId, src => src.MapFrom(x => x.KitchenId))
                .ForMember(dest => dest.Unit, src => src.MapFrom(x => Library.Generic.Utility.GetEnumStringValue<Unit>(x.Unit)));

            CreateMap<MenuItem, MenuItemDTO>()
                .ForMember(dest => dest.MenuItemId, src => src.MapFrom(x => x.MenuItemId))
                .ForMember(dest => dest.CategoryId, src => src.MapFrom(x => x.CategoryId))
                .ForMember(dest => dest.Price, src => src.MapFrom(x => x.Price))
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Name))
                .ForMember(dest => dest.Description, src => src.MapFrom(x => x.Description))
                .ForMember(dest => dest.KitchenId, src => src.MapFrom(x => x.KitchenId))
                .ForMember(dest => dest.IsAvalilable, src => src.MapFrom(x => x.IsAvalilable));

            CreateMap<InventoryMovement, InventoryMovementDTO>()
                .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id))
                .ForMember(dest => dest.MovementDate, src => src.MapFrom(x => x.MovementDate))
                .ForMember(dest => dest.Quantity, src => src.MapFrom(x => x.Quantity))
                .ForMember(dest => dest.InventoryId, src => src.MapFrom(x => x.InventoryId))
                .ForMember(dest => dest.KitchenId, src => src.MapFrom(x => x.KitchenId));
        }
    }
}
