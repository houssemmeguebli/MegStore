using MegStore.Core.Entities.ProductFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.DTOs
{
    public class ProductDto
    {
        public long productId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public IList<string>? ImageUrls { get; set; } = new List<string>();


        public int StockQuantity { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool IsAvailable { get; set; }
        public long? CategoryId { get; set; }
        public long? AdminId { get; set; }
        public int ItemQuantiy { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }

    }
}
