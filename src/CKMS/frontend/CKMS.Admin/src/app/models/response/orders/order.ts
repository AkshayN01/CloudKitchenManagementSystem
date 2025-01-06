export interface OrderList{
    orders: OrderListDTO[],
    totalCount: number
}

export interface OrderListDTO{
    orderId: string,
    kitchenName: string,
    orderDate: string,
    orderStatus: string,
    netAmount: number,
    itemCount: number
}

export interface OrderDetail{
    orderId : string,
    customerName: string,
    address: string,
    discountCouponCode: string,
    grossAmount: number,
    netAmount: number,
    status: string,
    paymentStatus: string,
    orderDate: string,
    inProgressDate: string,
    outForDeliveryDate: string,
    deliveredDate: string,
    items: OrderItem[]
}

export interface OrderItem{
    orderId: string,
    menuItemId: string,
    itemName: string,
    quantity: number
}