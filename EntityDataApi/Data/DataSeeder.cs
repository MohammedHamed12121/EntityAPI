using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityDataApi.Models;

namespace EntityDataApi.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // Check if there is already data in the database
            if (_context.Entities.Any())
            {
                Console.WriteLine("Database already seeded. Skipping seeding operation.");
                return;
            }

            var entities = new List<Entity>
            {
                new Entity
                {
                    Id = "1",
                    Deceased = false,
                    Gender = "Male",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "123 Main St", City = "Cityville", Country = "USA" },
                        new Address { AddressLine = "456 Elm St", City = "Townsville", Country = "USA" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "Birth", DateValue = new DateTime(1980, 1, 1) },
                        new Date { DateType = "Death", DateValue = new DateTime(2050, 12, 31) }
                    },
                    Names = new List<Name>
                    {
                        new Name { FirstName = "John", MiddleName = "M", Surname = "Doe" }
                    }
                },
                new Entity
                {
                    Id = "2",
                    Deceased = false,
                    Gender = "Female",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "789 Oak St", City = "Villageton", Country = "USA" },
                        new Address { AddressLine = "101 Pine St", City = "Townsville", Country = "USA" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "Birth", DateValue = new DateTime(1990, 5, 15) },
                        new Date { DateType = "Death", DateValue = new DateTime(2075, 7, 20) }
                    },
                    Names = new List<Name>
                    {
                        new Name { FirstName = "Jane", MiddleName = "A", Surname = "Smith" }
                    }
                },
                new Entity
                {
                    Id = "3",
                    Deceased = false,
                    Gender = "Male",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "321 Oak St", City = "Villageton", Country = "USA" },
                        new Address { AddressLine = "555 Maple St", City = "Towntown", Country = "USA" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "Birth", DateValue = new DateTime(1975, 8, 20) },
                        new Date { DateType = "Death", DateValue = new DateTime(2060, 6, 10) }
                    },
                    Names = new List<Name>
                    {
                        new Name { FirstName = "Michael", MiddleName = "J", Surname = "Johnson" }
                    }
                },
                new Entity
                {
                    Id = "4",
                    Deceased = true,
                    Gender = "Female",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "777 Willow St", City = "Ruraltown", Country = "USA" },
                        new Address { AddressLine = "999 Elm St", City = "Cityville", Country = "USA" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "Birth", DateValue = new DateTime(1950, 3, 5) },
                        new Date { DateType = "Death", DateValue = new DateTime(2020, 10, 15) }
                    },
                    Names = new List<Name>
                    {
                        new Name { FirstName = "Emily", MiddleName = "S", Surname = "Williams" }
                    }
                }
            };

            _context.Entities.AddRange(entities);
            _context.SaveChanges();
            Console.WriteLine("Sample data seeded successfully.");
        }
    }
}