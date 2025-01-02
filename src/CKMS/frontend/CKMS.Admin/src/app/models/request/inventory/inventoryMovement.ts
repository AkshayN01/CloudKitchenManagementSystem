export interface InventoryMovementPayload{
    inventoryId: number,
    movementType: number,
    quantity: number,
    movementDate: string
}

export interface InventoryMovementUpdatePayload{
    inventoryMovementId: number,
    inventoryId: number,
    movementType: number,
    quantity: number,
    movementDate: string
}