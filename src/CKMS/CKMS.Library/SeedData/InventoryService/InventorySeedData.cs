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
    public class InventoryData
    {
        public String Name { get; set; } = String.Empty!;
        public float Quantity { get; set; }
        public int Unit { get; set; }
        public float RestockThreshold { get; set; }
        public float MaxStockLevel { get; set; }
        public int MovementFrequency { get; set; }
        public int MovementFrequencyValue { get; set; }
    }
    public enum InventoryMovementFrequency
    {
        Weekly = 0,
        Monthly = 1,
        Daily  = 2
    }
    public static class InventorySeedData
    {
        private static String InventoryFileName = "Inventory.json";
        private static String InventoryMovementFileName = "InventoryMovement.json";
        public static List<InventoryMovement>? Movements { get; set; }
        public static List<InventoryData> InventoryData { get; set; }
        public static List<Inventory>? Inventories { get; set; }
        public static DateTime MoveInDate = new DateTime(2024, 01, 01);
        public static DateTime EndDate = new DateTime(2024, 11, 30);

        public static async Task<List<Inventory>> GetInventories()
        {
            if(Inventories == null)
            {
                Inventories = await Utility.ReadFromFile<List<Inventory>>(InventoryFileName);
                if (Inventories != null && Inventories.Count > 0)
                    return Inventories;

                Inventories = new List<Inventory>();
                List<Kitchen> Kitchens = await KitchenSeedData.GetKitchenSeedData();
                List<InventoryData> indevtoryData = GetInventoryData();
                foreach (Kitchen kitchen in Kitchens) {
                    foreach (var item in indevtoryData.Select((value, i) => new { i, value }))
                    {
                        Inventory inventory = new Inventory()
                        {
                            CreatedAt = DateTime.UtcNow,
                            InventoryId = item.i + 1,
                            InventoryName = item.value.Name,
                            KitchenId = kitchen.KitchenId,
                            LastUpdatedAt = DateTime.UtcNow,
                            Quantity = 0,
                            RestockThreshold = item.value.RestockThreshold,
                            MaxStockLevel = item.value.MaxStockLevel,
                            Unit = item.value.Unit
                        };
                        Inventories.Add(inventory);
                    }
                }
                await Utility.WriteToFile<List<Inventory>>(InventoryFileName, Inventories);
            }
            return Inventories;
        }
        
        public static async Task<List<InventoryMovement>> GetInventoryMovements()
        {
            if(Movements == null)
            {
                Movements = await Utility.ReadFromFile<List<InventoryMovement>>(InventoryMovementFileName);
                if(Movements != null && Movements.Count > 0)
                    return Movements;

                Movements = new List<InventoryMovement>();
                List<InventoryData> inventoryData = GetInventoryData();
                List<Inventory> inventories = await GetInventories();
                int count = 1;
                foreach (Inventory inventory in inventories)
                {
                    InventoryData? data = inventoryData.FirstOrDefault(x => x.Name.Equals(inventory.InventoryName));
                    if(data != null)
                    {
                        DateTime dateTime = MoveInDate;
                        while(dateTime <= EndDate)
                        {
                            InventoryMovement movement = new InventoryMovement()
                            {
                                Id = count,
                                CreatedAt = TimeZoneInfo.ConvertTimeToUtc(dateTime),
                                InventoryId = inventory.InventoryId,
                                KitchenId = inventory.KitchenId,
                                MovementDate = TimeZoneInfo.ConvertTimeToUtc(dateTime),
                                Quantity = data.Quantity,
                            };
                            Movements.Add(movement);
                            dateTime = data.MovementFrequency switch
                            {
                                (int)InventoryMovementFrequency.Monthly => dateTime.AddMonths(data.MovementFrequencyValue),
                                (int)InventoryMovementFrequency.Weekly => dateTime.AddDays(data.MovementFrequencyValue * 7),
                                (int)InventoryMovementFrequency.Daily => dateTime.AddDays(data.MovementFrequencyValue),
                            };
                            count++;
                        }
                    }
                }

                await Utility.WriteToFile<List<InventoryMovement>>(InventoryMovementFileName, Movements);

            }
            return Movements;
        }
        private static List<InventoryData> GetInventoryData()
        {
            if(InventoryData == null)
            {
                InventoryData = new List<InventoryData>()
                {
                    new InventoryData() { Name = "Pizza Dough", Quantity = 15, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "All Purpose Flour", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 80, RestockThreshold = 10, MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Tomato", Quantity = 8, Unit = (int)Unit.kilograms, MaxStockLevel = 30, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Tomato Sauce", Quantity = 15, Unit = (int)Unit.kilograms, MaxStockLevel = 20, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Pizza Sauce", Quantity = 15, Unit = (int)Unit.kilograms, MaxStockLevel = 30, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Basil", Quantity = 3, Unit = (int)Unit.kilograms, MaxStockLevel = 5, RestockThreshold = 1, MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Olives", Quantity = 5, Unit = (int)Unit.kilograms, MaxStockLevel = 10, RestockThreshold = 1, MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Margherita cheese", Quantity = 15, Unit = (int)Unit.kilograms, MaxStockLevel = 20, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Shredded Cheese", Quantity = 15, Unit = (int)Unit.kilograms, MaxStockLevel = 20, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Bell Peppers", Quantity = 5, Unit = (int)Unit.kilograms, MaxStockLevel = 10, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Pepporini", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 30, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Onion", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 30, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Chicken", Quantity = 15, Unit = (int)Unit.kilograms, MaxStockLevel = 20, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Eggs", Quantity = 5, Unit = (int)Unit.dozen, MaxStockLevel = 20, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Beef", Quantity = 15, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Meatballs", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Pork", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Jalapeno", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Sausage", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Bun", Quantity = 5, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Slice Cheese", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Patty", Quantity = 15, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Lettuce", Quantity = 5, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Pickles", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Pita Bread", Quantity = 8, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Potato", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Tortilla", Quantity = 5, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Vegetable Oil", Quantity = 20, Unit = (int)Unit.litres, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Olive Oil", Quantity = 20, Unit = (int)Unit.litres, MaxStockLevel = 50, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Butter", Quantity = 10, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Black Pepper Grounded", Quantity = 1, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Cajun powder", Quantity = 1, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Onion Powder", Quantity = 1, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                    new InventoryData() { Name = "Garlic", Quantity = 5, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Weekly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Garlic Powder", Quantity = 1, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Salt", Quantity = 2, Unit = (int)Unit.kilograms, MaxStockLevel = 50, RestockThreshold = 5, MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1 },
                    new InventoryData() { Name = "Tissues", Quantity = 30, Unit = (int)Unit.count, MaxStockLevel = 50, RestockThreshold = 5 , MovementFrequency = (int)InventoryMovementFrequency.Monthly, MovementFrequencyValue = 1},
                };
            }

            return InventoryData;
        }
    }
}
