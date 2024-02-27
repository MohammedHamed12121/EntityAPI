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

            // Sample generation
            var entity = new Entity
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
            };

            // Add entity to the context and save changes
            _context.Entities.Add(entity);
            _context.SaveChanges();

            Console.WriteLine("Sample data seeded successfully.");
        }
    }
}