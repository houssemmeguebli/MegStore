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

        public long? CustomerId { get; set; }
        public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
 
}
