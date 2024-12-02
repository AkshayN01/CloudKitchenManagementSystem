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
        private static List<DiscountUsage> DiscountUsages { get; set; }
        private static List<Order> Orders { get; set; }
        private static List<OrderItem> OrderItems { get; set; }
        private static List<Discount> Discounts { get; set; }
        private static List<Payment> Payment { get; set; }
        public static List<Order> GetOrders()
        {
            if(Orders == null)
            {
                Random random = new Random();
                Orders = new List<Order>();
                DiscountUsages = new List<DiscountUsage>();
                OrderItems = new List<OrderItem>();
                Payment = new List<Payment>();
                //20-50 orders per day
                //during discount week orders need to increase and sometimes remain the same depending on the percentage
                List<Kitchen> kitchens = KitchenSeedData.GetKitchenSeedData();
                //int orderCount = random.Next(20, 50);
                Discounts = DiscountSeedData.GetDiscounts();
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

                        List<Customer> customerList = PickRandomCustomers(orderCount);
                        foreach (Customer customer in customerList)
                        {
                            DateTime orderDate = startDate.AddMinutes(random.Next(0, 780)); //picks time between 11am and 11pm
                            List<MenuItem> menu = GetRandomMenuItem(random.Next(1, 6));
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
                                OrderId = Guid.NewGuid(),
                                Status = (int)OrderStatus.placed
                            };

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
                                PaymentStatus = (int)PaymentStatus.pending,
                            };
                            Payment.Add(payment);
                        }
                        startDate = startDate.AddDays(1);
                    }
                }
            }
            return Orders;
        }
        public static List<OrderItem> GetOrderItems()
        {
            return OrderItems;
        }
        public static List<DiscountUsage> GetDiscountUsages()
        {
            return DiscountUsages;
        }
        public static List<Payment> GetPayments()
        {
            return Payment;
        }
        private static Discount? CheckDiscountDate(DateTime dateTime)
        {
            return Discounts.FirstOrDefault(x => dateTime >= x.StartDate && dateTime <= x.EndDate);
        }
        private static List<Customer> PickRandomCustomers(int countToPick)
        {
            List<Customer> customers = CustomerSeedData.GetCustomers();
            Random random = new Random();

            // Shuffle the list
            List<Customer> shuffledList = customers.OrderBy(x => random.Next()).ToList();

            // Take the first N elements
            return shuffledList.Take(countToPick).ToList();
        }
        private static List<MenuItem> GetRandomMenuItem(int countToPick)
        {
            List<MenuItem> menuItemList = MenuItemSeedData.GetMenuItems();
            Random random = new Random();

            List<MenuItem> shuffledList = menuItemList.OrderBy(x => random.Next()).ToList();

            return shuffledList.Take(countToPick).ToList();
        }
    }
}
