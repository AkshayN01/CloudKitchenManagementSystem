export interface OrderItemDTO{
    orderId: string,
    menuItemId: number,
    itemName: string,
    quantity: number
}

export interface OrderDTO{
    orderId: string,
    customerId: string,
    orderDate: string,
    inProgressDate: string,
    outForDeliveryDate: string,
    deliveredDate: string,
    netAmount: number,
    grossAmount: number,
    status: string,
    address: string,
    paymentStatus: string,
    items: OrderItemDTO[]
}

export interface OrderListDTO{
    orderId: string,
    kitchenName: string,
    itemCount: number,
    netAmount: number,
    orderStatus: string,
    orderDate: string
}

export interface OrderList{
    orders: OrderListDTO[],
    totalCount: number
}

export interface OrderCartDTO{
    orderId: string,
    customerId: string,
    orderDate: string,
    netAmount: number,
    grossAmount: number,
    items: OrderItemCartDTO[]
}

export interface OrderItemCartDTO{
    menuItemId: number,
    itemName: string,
    quantity: number
}