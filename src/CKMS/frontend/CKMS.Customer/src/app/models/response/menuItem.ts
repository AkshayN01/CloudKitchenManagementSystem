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