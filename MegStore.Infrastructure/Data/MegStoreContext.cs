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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MegStoreContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

    }
}
