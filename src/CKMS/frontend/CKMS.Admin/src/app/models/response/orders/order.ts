export interface OrderList{
    orders: OrderListDTO[],
    totalNumber: number
}

export interface OrderListDTO{
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
    items: OrderItem[]
}

export interface OrderItem{
    orderId: string,
    menuItemId: string,
    itemName: string,
    quantity: number
}