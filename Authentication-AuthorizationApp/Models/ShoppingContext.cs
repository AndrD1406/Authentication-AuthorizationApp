using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TaskAuthenticationAuthorization.Models
{
    public class ShoppingContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SuperMarket> SuperMarkets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public ShoppingContext(DbContextOptions<ShoppingContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "user" },
                new Role { Id = 2, Name = "admin" },
                new Role { Id = 3, Name = "buyer" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "admin@gmail.com", Password = "12345678", RoleId = 2,
                    FirstName = "Ostap",
                    LastName = "Bender",
                    Address = "Rio de Zhmerinka",
                    Discount = Discount.O,
                },
                new User { Id = 2, Email = "user@gmail.com", Password = "12345678", RoleId = 1,
                    FirstName = "Shura",
                    LastName = "Balaganov",
                    Address = "Odessa",
                    Discount = Discount.R,
                },
                new User { Id = 3, Email = "buyer@gmail.com", Password = "12345678", RoleId = 3,
                    FirstName = "Shura",
                    LastName = "Balaganov",
                    Address = "Odessa",
                    Discount = Discount.R,
                }
            );
        }
    }
}
