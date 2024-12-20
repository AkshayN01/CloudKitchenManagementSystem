export interface OrderList{
    orders: OrderListDTO[],
    pageNumber: number,
    pageSize: number
}

export interface OrderListDTO{
    kitchenName: string,
    orderDate: string,
    orderStatus: string,
    netAmount: number,
    itemCount: number
}