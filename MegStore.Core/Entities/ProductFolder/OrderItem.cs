using MegStore.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Entities.ProductFolder
{
    public class OrderItem
    {
        public long OrderItemId { get; set; }

        [ForeignKey(nameof(Product))]
        public long? ProductId { get; set; }
        public Product? Product { get; set; }
        [ForeignKey(nameof(Order))]
        public long? OrderId { get; set; }
        public Order? Order { get; set; }
       
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
