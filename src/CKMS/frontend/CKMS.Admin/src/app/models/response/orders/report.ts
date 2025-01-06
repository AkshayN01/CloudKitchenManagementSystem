export interface OrderReportSummary{
    totalOrders: number,
    totalDiscountedOrders: number,
    netRevenue: number,
    grossRevenue: number,
    avgOrderValue: number,
    totalExpense: number,
    orderingPatterns: CustomerOrderingPattern[]
}

export interface DiscountEffectiveness{
    totalOrders: number,
    discountedOrders: number,
    revenueFromDiscountedOrders: number,
    revenueFromNonDiscountedOrders: number,
    averageOrderValueWithDiscount: number,
    averageOrderValueWithoutDiscount: number,
    discountUsagePercentage: number
}

export interface BestSellingDish{
    menuItemName: string,
    menuItemId: number,
    orderCount: number,
    totalQuantity: number
}

export interface TopCustomers{
    customerName: string,
    customerId: string,
    totalOrders: number
}

export interface CustomerSummary{
    customerName: string,
    customerId: string,
    totalOrders: number,
    totalDiscountedOrders: number,
    netRevenue: number,
    grossRevenue: number,
    avgOrderValue: number,
    latestOrders: LatestOrder[],
    preferredDish: PreferredDish,
    orderingPatterns: CustomerOrderingPattern[]
}

export interface PreferredDish{
    menuItemId: number,
    menuItemName: string,
    totalQuantity: number,
    timePeriod: string
}

export interface CustomerOrderingPattern{
    timePeriod: string,
    ordersCount: number
}

export interface LatestOrder{
    orderId: string,
    netAmount: number,
    orderDate: string,
    status: string,
    itemCount: number,
    items: OrderItemSummary[]
}

export interface OrderItemSummary{
    menuItemName: string,
    totalQuantity: number
}