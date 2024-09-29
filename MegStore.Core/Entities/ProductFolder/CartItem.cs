using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Entities.ProductFolder
{
    public class CartItem
    {
        public long CartItemId { get; set; } // Primary Key

        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; } 
        public Cart? Cart { get; set; } 
        [ForeignKey(nameof(Product))]
        public long ProductId { get; set; } 
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } 
        public decimal? TotalPrice {  get; set; }
    }
}

