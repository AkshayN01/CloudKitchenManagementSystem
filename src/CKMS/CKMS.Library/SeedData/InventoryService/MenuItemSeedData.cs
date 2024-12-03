using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Library.Generic;
using CKMS.Library.SeedData.AdminUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.SeedData.InventoryService
{
    public static class MenuItemSeedData
    {
        private static String CategoryFileName = "Category.json";
        private static String MenuItemFileName = "MenuItem.json";
        public static List<Category> Categories { get; set; }
        public static List<MenuItem> Items { get; set; }
        public static Dictionary<String, List<String>> Menu = new Dictionary<string, List<string>>()
        {
            { "Pizza", new List<string>(){ "Chicken Pizza", "Chicken Pepporini Pizza", "Meatball Pizza", "Sausage Pizza", "Veggie Pizza", "All Meat Pizza" } },
            { "Burger", new List<string>(){ "Chicken Burger", "Fried Chicken Burger", "Double Cheese Burger", "Cheese Burger", "Tower Burger"} },
            { "Kebabs", new List<string>(){ "Doner Kebab", "Chicken Filet Kebab", "Doner Kebab Tray", "Chicken Filet Kebab Tray"} },
            { "Breakfast", new List<string>(){ "English Breakfast", "Pancakes and Sausages" } },
            { "Sides", new List<string>(){ "BBQ Chicken Wings", "Hot Chicken Wings", "Fries", "Cheesy Fries", "Fried Chicken tenders"} },
            { "Meal Deal", new List<string>(){ "Chicken Pizza Deal", "Chicken Pepporini Pizza Deal", "Meatball Pizza Deal", "Sausage Pizza Deal", "Veggie Pizza Deal", "All Meat Pizza Deal",
                "Chicken Burger Meal", "Fried Chicken Burger Meal", "Double Cheese Burger Meal", "Tower Burger Meal"
            } }
        };

        public static async Task<List<Category>> GetCategories()
        {
            if(Categories == null)
            {
                Categories = await Utility.ReadFromFile<List<Category>>(CategoryFileName);
                if(Categories != null && Categories.Count > 0)
                    return Categories;

                Categories = new List<Category>();
                List<Kitchen> kitchens = await KitchenSeedData.GetKitchenSeedData();
                foreach (Kitchen k in kitchens) {
                    foreach (var item in Menu.Select((value, i) => new { value, i }))
                    {
                        Category category = new Category()
                        {
                            CategoryId = item.i + 1,
                            CreatedAt = DateTime.UtcNow,
                            Description = "",
                            KitchenId = k.KitchenId,
                            Name = item.value.Key,
                        };
                        Categories.Add(category);
                    }
                }
                await Utility.WriteToFile<List<Category>>(CategoryFileName, Categories);
            }
            return Categories;
        }

        public static async Task<List<MenuItem>> GetMenuItems()
        {
            if(Items == null)
            {
                Items = await Utility.ReadFromFile<List<MenuItem>>(MenuItemFileName);
                if(Items != null && Items.Count > 0)
                    return Items;

                Random random = new Random();
                Items = new List<MenuItem>();
                List<Category> categories = await GetCategories();
                int count = 1;
                foreach (var item in categories)
                {
                    (int start, int end) = item.Name switch
                    {
                        "Pizza" => (9, 15),
                        "Burger" => (3, 11),
                        "Kebabs" => (5, 10),
                        "Breakfast" => (8, 10),
                        "Sides" => (3, 9),
                        "Meal Deal" => (14, 20)
                    };
                    KeyValuePair<String, List<String>> keyValuePair = Menu.Where(k => k.Key == item.Name).FirstOrDefault();
                    foreach(String name in keyValuePair.Value)
                    {
                        MenuItem menuItem = new MenuItem()
                        {
                            CreatedAt = DateTime.UtcNow,
                            CategoryId = item.CategoryId,
                            Name = name,
                            Description = "",
                            IsAvalilable = 1,
                            KitchenId = item.KitchenId,
                            Price = random.Next(start, end),
                            MenuItemId = count,
                        };
                        count++;
                        Items.Add(menuItem);
                    }
                }
                await Utility.WriteToFile<List<MenuItem>>(MenuItemFileName, Items);
            }
            return Items;
        }
    }
}
