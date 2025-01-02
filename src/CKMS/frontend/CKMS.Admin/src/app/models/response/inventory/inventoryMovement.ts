export interface InventoryMovementListDTO{
    totalCount: number,
    inventoryMovements: InventoryMovementDTO[]
}

export interface InventoryMovementDTO{
    id: number,
    inventoryId: number,
    kitchenId: string,
    movementType: number,
    quantity: number,
    movementDate: string
}