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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // one-to-many relationship (Entity and Address)
            modelBuilder.Entity<Entity>()
                    .HasMany(e => e.Addresses)
                    .WithOne(a => a.Entity)
                    .HasForeignKey(a => a.EntityId)
                    .IsRequired();

            // one-to-many relationship (Entity and Date)
            modelBuilder.Entity<Entity>()
                    .HasMany(e => e.Dates)
                    .WithOne(d => d.Entity)
                    .HasForeignKey(d => d.EntityId)
                    .IsRequired();

            // one-to-many relationship (Entity and Name)
            modelBuilder.Entity<Entity>()
                    .HasMany(e => e.Names)
                    .WithOne(n => n.Entity)
                    .HasForeignKey(n => n.EntityId)
                    .IsRequired();
        }

    }
}