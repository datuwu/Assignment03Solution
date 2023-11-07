using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessObject
{
    public class EstoreContext : IdentityDbContext<User>
    {
        public EstoreContext()
        {
        }

        public EstoreContext(DbContextOptions<EstoreContext> options) : base(options) 
        { 
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            optionsBuilder.LogTo(Console.WriteLine).EnableSensitiveDataLogging();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, MemberId = 1, OrderDate = new DateTime(2021, 1, 1), RequiredDate = new DateTime(2021, 2, 1), ShippedDate = new DateTime(2021, 3, 1), Freight = 100 },
                new Order { OrderId = 2, MemberId = 2, OrderDate = new DateTime(2021, 2, 1), RequiredDate = new DateTime(2021, 3, 1), ShippedDate = new DateTime(2021, 4, 1), Freight = 200 },
                new Order { OrderId = 3, MemberId = 3, OrderDate = new DateTime(2021, 3, 1), RequiredDate = new DateTime(2021, 4, 1), ShippedDate = new DateTime(2021, 5, 1), Freight = 300 },
                new Order { OrderId = 4, MemberId = 4, OrderDate = new DateTime(2021, 4, 1), RequiredDate = new DateTime(2021, 5, 1), ShippedDate = new DateTime(2021, 6, 1), Freight = 400 },
                new Order { OrderId = 5, MemberId = 5, OrderDate = new DateTime(2021, 5, 1), RequiredDate = new DateTime(2021, 6, 1), ShippedDate = new DateTime(2021, 7, 1), Freight = 500 }
            );

            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail { OrderId = 1, ProductId = 1, UnitPrice = 200, Quantity = 10, Discount = 50 },
                new OrderDetail { OrderId = 2, ProductId = 2, UnitPrice = 300, Quantity = 15, Discount = 50 },
                new OrderDetail { OrderId = 3, ProductId = 3, UnitPrice = 400, Quantity = 20, Discount = 50 },
                new OrderDetail { OrderId = 4, ProductId = 4, UnitPrice = 500, Quantity = 25, Discount = 50 },
                new OrderDetail { OrderId = 5, ProductId = 5, UnitPrice = 600, Quantity = 30, Discount = 50 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, CategoryId = 1, ProductName = "Bouquet", Weight = 10, UnitPrice = 450, UnitsInStock = 15 },
                new Product { ProductId = 2, CategoryId = 2, ProductName = "Book", Weight = 5, UnitPrice = 350, UnitsInStock = 12 },
                new Product { ProductId = 3, CategoryId = 3, ProductName = "Toy Car", Weight = 20, UnitPrice = 800, UnitsInStock = 15 },
                new Product { ProductId = 4, CategoryId = 4, ProductName = "Popcorn", Weight = 11, UnitPrice = 250, UnitsInStock = 18 },
                new Product { ProductId = 5, CategoryId = 5, ProductName = "Keychain", Weight = 5, UnitPrice = 100, UnitsInStock = 13 }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Valentine"},
                new Category { CategoryId = 2, CategoryName = "Study" },
                new Category { CategoryId = 3, CategoryName = "Toy" },
                new Category { CategoryId = 4, CategoryName = "Food" },
                new Category { CategoryId = 5, CategoryName = "Souvenir" }
            );

            modelBuilder.Entity<OrderDetail>().HasKey(e => new { e.OrderId, e.ProductId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
