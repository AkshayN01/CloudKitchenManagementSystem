using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.SeedData.CustomerService
{
    public static class CustomerSeedData
    {
        public static List<Customer>? Customers { get; set; }
        public static List<Address>? Addresses { get; set; }
        public static List<Customer> GetCustomers()
        {
            if (Customers == null)
            {
                Random random = new Random();
                Customers = new List<Customer>();
                Addresses = new List<Address>();
                HashSet<string> usedEmails = new HashSet<string>();
                HashSet<string> usedPhoneNumbers = new HashSet<string>();

                for (int i = 0; i < 500; i++)
                {
                    string name = GenerateRandomName(random);
                    string email;
                    do
                    {
                        email = GenerateRandomEmail(name, random);
                    } while (!usedEmails.Add(email)); // Ensures uniqueness

                    string phone;
                    do
                    {
                        phone = GenerateRandomPhoneNumber(random);
                    } while (!usedPhoneNumbers.Add(phone)); // Ensures uniqueness

                    Guid customerId = Guid.NewGuid();
                    Address address = GenerateRandomDublinAddress(random, customerId);
                    Addresses.Add(address);
                    Customer customer = new Customer()
                    {
                        AddressList = new List<Address>() { address },
                        CreatedAt = DateTime.UtcNow,
                        CustomerId = customerId,
                        EmailId = email,
                        Name = name,
                        PasswordHash = PasswordHasher.HashPassword("admin1234"),
                        PhoneNumber = phone,
                        UserName = email,
                    };
                    Customers.Add(customer);
                }
            }
            return Customers;
        }
        public static List<Address> GetAddresses()
        {
            return Addresses;
        }
        static string GenerateRandomName(Random random)
        {
            string[] firstNames = { "John", "Jane", "Mike", "Sarah", "Chris", "Emma", "David", "Sophia", "James", "Olivia" };
            string[] lastNames = { "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Lee", "Martin", "White", "Clark", "Harris" };

            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];

            return $"{firstName} {lastName}";
        }

        static string GenerateRandomEmail(string name, Random random)
        {
            string[] domains = { "gmail.com", "yahoo.com", "outlook.com", "example.com", "hotmail.com" };
            string domain = domains[random.Next(domains.Length)];

            string sanitizedName = name.ToLower().Replace(" ", ".");
            return $"{sanitizedName}{random.Next(1, 100000)}@{domain}";
        }

        static string GenerateRandomPhoneNumber(Random random)
        {
            return $"({random.Next(100, 1000)}) {random.Next(100, 1000)}-{random.Next(1000, 10000)}";
        }
        static Address GenerateRandomDublinAddress(Random random, Guid CustomerId)
        {
            string[] regions = { "North Dublin", "South Dublin", "Dublin City Centre", "Dublin Bay Area" };
            string[] streets = {
            "O'Connell Street", "Grafton Street", "Dame Street", "Talbot Street", "Capel Street",
            "Baggot Street", "Pearse Street", "Amiens Street", "Ranelagh Road", "Phibsborough Road"
        };
            string[] postalCodes = { "D01", "D02", "D03", "D04", "D06", "D07", "D08", "D09", "D10", "D12" };

            string region = regions[random.Next(regions.Length)];
            string street = streets[random.Next(streets.Length)];
            string postalCode = postalCodes[random.Next(postalCodes.Length)] + random.Next(1, 100).ToString("D2");

            return new Address
            {
                Region = region,
                AddressDetail = street,
                PostalCode = postalCode,
                AddressId = Guid.NewGuid(),
                City = "Dublin",
                Country = "Ireland",
                CustomerId = CustomerId
            };
        }

    }
}
