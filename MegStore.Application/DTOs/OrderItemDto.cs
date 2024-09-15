using MegStore.Core.Entities.ProductFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.DTOs
{
    public class OrderItemDto
    {
        public long OrderItemId { get; set; }
  
        public long ProductId { get; set; }
        public long OrderId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
