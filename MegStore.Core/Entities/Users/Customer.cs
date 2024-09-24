using MegStore.Core.Entities.ProductFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Entities.Users
{
    public class Customer : User
    {
        public ICollection<Order> Orders { get; set; }

    }
}
