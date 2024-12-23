export interface InventoryPayload{
    inventoryName: string,
    quantity: number,
    unit: number,
    restockThreshold: number,
    maxStockLevel: number
}

export interface InventoryUpdatePayload{
    inventoryId: number,
    inventoryName: string,
    quantity: number,
    unit: number,
    restockThreshold: number,
    maxStockLevel: number
}