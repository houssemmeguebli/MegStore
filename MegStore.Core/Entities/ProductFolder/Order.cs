using MegStore.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Entities.ProductFolder
{
    public enum OrderStatus
    {
        Pending,
        Shipped, 
        Rejected
    }
    public class Order
    {
        public long orderId { get; set; }
        public DateTime orderDate { get; set; } = DateTime.Now;
        public DateTime? shippedDate { get; set; }

        public OrderStatus orderStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; } 
        public string orderNotes { get; set; }
        public decimal TotlaAmount { get; set; }
        public int TotalProducts { get; set; }
        public int Quantity { get; set; }

        [ForeignKey(nameof(Category))]
        public long? customerId { get; set; }
        public Customer? customer { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }


    }
}
