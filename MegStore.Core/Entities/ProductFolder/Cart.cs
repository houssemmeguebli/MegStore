using MegStore.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        public ICollection<CartItem>? CartItems { get; set; }

        [ForeignKey(nameof(Category))]
        public long? customerId { get; set; }
        public Customer? customer { get; set; }
        public decimal TotalAmount { get; set; }

    }
}
