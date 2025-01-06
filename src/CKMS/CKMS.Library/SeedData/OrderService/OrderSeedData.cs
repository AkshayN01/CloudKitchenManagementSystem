using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Contracts.DBModels.OrderService;
using CKMS.Library.Generic;
using CKMS.Library.SeedData.AdminUserService;
using CKMS.Library.SeedData.CustomerService;
using CKMS.Library.SeedData.InventoryService;

namespace CKMS.Library.SeedData.OrderService
{
    public static class OrderSeedData
    {
        private static List<Customer> CustomerSeedList = new List<Customer>();
        private static List<MenuItem> MenuItemSeedList = new List<MenuItem>();
        private static String DiscountUsageFileName = "DiscountUsage.json";
        private static String OrdersFileName = "Orders.json";
        private static String OrderItemsFileName = "OrderItems.json";
        private static String PaymentFileName = "Payments.json";
        private static List<DiscountUsage> DiscountUsages { get; set; }
        private static List<Order> Orders { get; set; }
        private static List<OrderItem> OrderItems { get; set; }
        private static List<Discount> Discounts { get; set; }
        private static List<Payment> Payment { get; set; }
        public static async Task<List<Order>> GetOrders()
        {
            if(Orders == null)
            {
                Orders = await Utility.ReadFromFile<List<Order>>(OrdersFileName);
                if(Orders != null && Orders.Count > 0)
                    return Orders;

                Random random = new Random();
                Orders = new List<Order>();
                DiscountUsages = new List<DiscountUsage>();
                OrderItems = new List<OrderItem>();
                Payment = new List<Payment>();
                //20-50 orders per day
                //during discount week orders need to increase and sometimes remain the same depending on the percentage
                List<Kitchen> kitchens = await KitchenSeedData.GetKitchenSeedData();
                //int orderCount = random.Next(20, 50);
                Discounts = await DiscountSeedData.GetDiscounts();
                DateTime startDate = new DateTime(2024, 01, 1, 11, 00, 00);
                DateTime endDate = new DateTime(2024, 11, 30, 23, 00, 00);
                foreach (Kitchen kitchen in kitchens) {
                    while(startDate <= endDate)
                    {
                        int orderCount = 0;
                        Discount? discount = CheckDiscountDate(startDate);
                        if (discount != null)
                        {
                            if (discount.DiscountValue >= 9)
                                orderCount = random.Next(40, 70);
                            else
                                orderCount = random.Next(20, 70);
                        }
                        else
                            orderCount = random.Next(20, 50);

                        List<Customer> customerList = await PickRandomCustomers(orderCount);
                        foreach (Customer customer in customerList)
                        {
                            DateTime orderDate = TimeZoneInfo.ConvertTimeToUtc(startDate.AddMinutes(random.Next(0, 780))); //picks time between 11am and 11pm
                            List<MenuItem> menu = await GetRandomMenuItem(random.Next(1, 6));
                            Double grossAmount = menu.Sum(x => x.Price);
                            Order order = new Order()
                            {
                                Address = customer.AddressList.First().AddressId,
                                CreatedAt = orderDate,
                                CustomerId = customer.CustomerId,
                                KitchenId = kitchen.KitchenId,
                                GrossAmount = grossAmount,
                                NetAmount = grossAmount,
                                OrderDate = orderDate,
                                InProgressTime = orderDate.AddMinutes(random.Next(1, 3)),
                                OrderId = Guid.NewGuid(),
                                Status = (int)OrderStatus.delivered
                            };
                            order.OutForDeliveryTime = order.InProgressTime.Value.AddMinutes(random.Next(20, 30));
                            order.DeliveryTime = order.OutForDeliveryTime.Value.AddMinutes(random.Next(15, 30));
                            order.UpdatedAt = order.DeliveryTime.Value;
                            Orders.Add(order);

                            foreach (MenuItem menuItem in menu)
                            {
                                OrderItem orderItem = new OrderItem()
                                {
                                    MenuItemId = menuItem.MenuItemId,
                                    OrderId = order.OrderId,
                                    OrderItemId = Guid.NewGuid(),
                                    Quantity = random.Next(1, 3),
                                };
                                OrderItems.Add(orderItem);
                            }

                            if (discount != null)
                            {
                                order.NetAmount = grossAmount - ((grossAmount * discount.DiscountValue) / 100);
                                DiscountUsage discountUsage = new DiscountUsage()
                                {
                                    CreatedAt = orderDate,
                                    DiscountId = discount.DiscountId,
                                    IsApplied = 1,
                                    OrderId = order.OrderId,
                                    UsageId = Guid.NewGuid(),
                                    UserId = customer.CustomerId,
                                };
                                DiscountUsages.Add(discountUsage);
                            }
                            Payment payment = new Payment()
                            {
                                Amount = order.NetAmount,
                                OrderId = order.OrderId,
                                PaymentDate = orderDate,
                                PaymentId = Guid.NewGuid(),
                                PaymentMethod = (int)PaymentMethod.CashOnDelivery,
                                PaymentStatus = (int)PaymentStatus.paid,
                            };
                            Payment.Add(payment);
                        }
                        startDate = startDate.AddDays(1);
                    }
                }
                await Utility.WriteToFile<List<Order>>(OrdersFileName, Orders);
                await Utility.WriteToFile<List<OrderItem>>(OrderItemsFileName, OrderItems);
                await Utility.WriteToFile<List<DiscountUsage>>(DiscountUsageFileName, DiscountUsages);
                await Utility.WriteToFile<List<Payment>>(PaymentFileName, Payment);
            }
            return Orders;
        }
        public static async Task<List<OrderItem>> GetOrderItems()
        {
            if (OrderItems == null)
                OrderItems = await Utility.ReadFromFile<List<OrderItem>>(OrderItemsFileName);

            return OrderItems;
        }
        public static async Task<List<DiscountUsage>> GetDiscountUsages()
        {
            if (DiscountUsages == null)
                DiscountUsages = await Utility.ReadFromFile<List<DiscountUsage>>(DiscountUsageFileName);

            return DiscountUsages;
        }
        public static async Task<List<Payment>> GetPayments()
        {
            if (Payment == null)
                Payment = await Utility.ReadFromFile<List<Payment>>(PaymentFileName);

            return Payment;
        }
        private static Discount? CheckDiscountDate(DateTime dateTime)
        {
            return Discounts.FirstOrDefault(x => dateTime >= x.StartDate && dateTime <= x.EndDate);
        }
        private static async Task<List<Customer>> PickRandomCustomers(int countToPick)
        {
            if (CustomerSeedList == null || CustomerSeedList.Count == 0)
                CustomerSeedList = await CustomerSeedData.GetCustomers();

            Random random = new Random();

            // Shuffle the list
            List<Customer> shuffledList = CustomerSeedList.OrderBy(x => random.Next()).ToList();

            // Take the first N elements
            return shuffledList.Take(countToPick).ToList();
        }
        private static async Task<List<MenuItem>> GetRandomMenuItem(int countToPick)
        {
            if(MenuItemSeedList == null || MenuItemSeedList.Count == 0)
                MenuItemSeedList = await MenuItemSeedData.GetMenuItems();

            Random random = new Random();

            List<MenuItem> shuffledList = MenuItemSeedList.OrderBy(x => random.Next()).ToList();

            return shuffledList.Take(countToPick).ToList();
        }
    }
}
