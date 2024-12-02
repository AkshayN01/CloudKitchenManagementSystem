using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DBModels.OrderService;
using CKMS.Library.SeedData.AdminUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.SeedData.OrderService
{
    public static class DiscountSeedData
    {
        private static List<Discount>? Discounts { get; set; }
        public static List<Discount> GetDiscounts()
        {
            if (Discounts == null)
            {
                Random random = new Random();
                Discounts = new List<Discount>();
                HashSet<string> generatedCodes = new HashSet<string>();
                var weeks = GetRandomWeeksFromEachMonth(2024, 1, 11);
                List<Kitchen> kitchens = KitchenSeedData.GetKitchenSeedData();
                foreach (var kitchen in kitchens)
                {
                    foreach (var week in weeks)
                    {
                        Discount discount = new Discount()
                        {
                            CouponCode = GenerateUniqueCouponCode(generatedCodes),
                            CreatedAt = week.StartDate.AddDays(-2),
                            StartDate = week.StartDate,
                            DiscountId = Guid.NewGuid(),
                            DiscountType = (int)DiscountType.Percentage,
                            DiscountValue = random.Next(5, 15),
                            EndDate = week.EndDate,
                            IsActive = 1,
                            IsPersonalised = 0,
                            KitchenId = kitchen.KitchenId,
                            UsageCount = 2,
                        };
                        Discounts.Add(discount);
                    }
                }
            }
            return Discounts;
        }
        private static (DateTime StartDate, DateTime EndDate, int Month)[] GetRandomWeeksFromEachMonth(int year, int startMonth, int endMonth)
        {
            Random random = new Random();
            var result = new (DateTime StartDate, DateTime EndDate, int Month)[endMonth - startMonth + 1];

            for (int month = startMonth; month <= endMonth; month++)
            {
                // Get the first and last day of the current month
                DateTime firstDayOfMonth = new DateTime(year, month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                // Generate a random day within the month
                int rangeDays = (lastDayOfMonth - firstDayOfMonth).Days;
                DateTime randomDate = firstDayOfMonth.AddDays(random.Next(rangeDays + 1));

                // Calculate the week for the random date
                DateTime startOfWeek = randomDate.AddDays(-(int)randomDate.DayOfWeek + (int)DayOfWeek.Monday);
                DateTime endOfWeek = startOfWeek.AddDays(6);

                // Ensure the week is within the bounds of the month
                if (startOfWeek < firstDayOfMonth) startOfWeek = firstDayOfMonth;
                if (endOfWeek > lastDayOfMonth) endOfWeek = lastDayOfMonth;

                result[month - startMonth] = (startOfWeek, endOfWeek, month);
            }

            return result;
        }
        private static string GenerateUniqueCouponCode(HashSet<string> existingCodes)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string couponCode;

            do
            {
                couponCode = new string(GenerateRandomCode(chars, random, 8));
            } while (existingCodes.Contains(couponCode)); // Ensure uniqueness

            existingCodes.Add(couponCode);
            return couponCode;
        }
        private static char[] GenerateRandomCode(string chars, Random random, int length)
        {
            char[] code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }
            return code;
        }
    }
}
