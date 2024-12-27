export interface OrderPayload{
    orderId: string,
    kitchenId: string,
    addressId: string,
    items: OrderItemPayload[]
}

export interface OrderItemPayload{
    menuItemId: number,
    quantity: number
}

export interface ConfirmOrderPayload{
    orderId: string,
    paymentMethod: number
}

export interface DiscountUsagePayload{
    couponCode: string,
    orderId: string,
    isApplied: number,
    appliedDate: string
}