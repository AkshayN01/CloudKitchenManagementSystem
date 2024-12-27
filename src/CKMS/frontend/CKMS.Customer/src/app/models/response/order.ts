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
    netAmount: number,
    grossAmount: number,
    status: string,
    address: string,
    paymentStatus: string,
    items: OrderItemDTO[]
}

export interface OrderListDTO{
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