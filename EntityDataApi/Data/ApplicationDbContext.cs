using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityDataApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityDataApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Entity> Entities { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Date> Dates { get; set; }
        public DbSet<Name> Names { get; set; }

    }
}