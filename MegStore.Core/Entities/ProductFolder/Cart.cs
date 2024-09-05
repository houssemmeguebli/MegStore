using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Entities.ProductFolder
{
    public class Cart
    {
        public int CartId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public IList<Product> Products { get; set; }= new List<Product>();
        public decimal TotalAmount { get; set; }

    }
}
