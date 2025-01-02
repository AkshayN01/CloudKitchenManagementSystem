export interface MenuItemDTO{
    menuItemId: number,
    kitchenId: string,
    name: string,
    description: string,
    price: number,
    categoryId: number,
    categoryName: number,
    isAvailable: number
}

export interface MenuItemListDTO{
    totalCount: number,
    menuItems: MenuItemDTO[]
}

export interface KitchenListDTO{
    totalCount: number,
    kitchenList: KitchenDTO[]
}

export interface KitchenDTO{
    kitchenId: string,
    kitchenName: string,
    address: string,
    city: string,
    region: string,
    postalCode: string,
    country: string
}