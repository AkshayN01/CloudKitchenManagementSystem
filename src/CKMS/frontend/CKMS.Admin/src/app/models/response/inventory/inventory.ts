export interface InventoryDTO{
    inventoryId: number,
    inventoryName: string,
    kitchenId: string,
    quantity: number,
    unit: string,
    restockThreshold: number,
    maxStockLevel: number
}

export interface InventoryListDTO{
    totalCount: number,
    inventories: InventoryDTO[]
}