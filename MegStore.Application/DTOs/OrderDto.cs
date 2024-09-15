using MegStore.Application.DTOs;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.DTOs
{
    public enum OrderStatus
    {
        Pending,
        Shipped,
        Rejected
    }
    public class OrderDto
    {
        public long OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? ShippedDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string orderNotes { get; set; }
        public decimal TotlaAmount { get; set; }
        public long? CustomerId { get; set; }
        public int Quantity { get; set; }
   
        public ICollection<OrderItem> OrderItems { get; set; }
    }
 
}
