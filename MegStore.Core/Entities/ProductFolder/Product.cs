using MegStore.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Entities.ProductFolder
{
    public class Product
    {
        public long productId { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; } 
        public decimal productPrice { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public int ItemQuantiy { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public bool IsAvailable { get; set; }

        [ForeignKey(nameof(Category))]
        public long? categoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey(nameof(admin))]
        public long? adminId { get; set; }
        public Admin? admin  { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }


    }
}
