export interface OrderPayload{
    orderId: string,
    kitchenId: string,
    items: OrderItemPayload[]
}

export interface OrderItemPayload{
    menuItemId: number,
    quantity: number
}

export interface ConfirmOrderPayload{
    orderId: string,
    paymentMethod: number,
    addressId: string,
}

export interface DiscountUsagePayload{
    couponCode: string,
    orderId: string,
    appliedDate: string
}