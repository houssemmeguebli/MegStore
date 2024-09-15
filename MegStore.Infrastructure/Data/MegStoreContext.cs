using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MegStore.Core.Entities.Users;
using MegStore.Core.Entities.ProductFolder;
namespace MegStore.Infrastructure.Data
{
    public class MegStoreContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }    
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public MegStoreContext(DbContextOptions<MegStoreContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MegStoreContext).Assembly);
            base.OnModelCreating(modelBuilder);
            // Apply column type configuration for Cart entity
            modelBuilder.Entity<Cart>()
                .Property(c => c.TotalAmount)
                .HasColumnType("decimal(18,2)");

            // Apply column type configuration for Product entity
            modelBuilder.Entity<Product>()
                .Property(p => p.productPrice)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>()
        .HasKey(o => o.orderId); // Primary Key

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.ClientNoAction); // Adjust the delete behavior as needed

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.OrderItemId); // Primary Key for OrderItem
            base.OnModelCreating(modelBuilder);
        }

    }

}

