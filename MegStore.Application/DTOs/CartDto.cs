using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.DTOs
{
    public class CartDto
    {
        public long CartId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
        public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
        public decimal TotalAmount { get; set; }
    }
}
