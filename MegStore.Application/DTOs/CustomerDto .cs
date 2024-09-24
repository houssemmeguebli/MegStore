using MegStore.Core.Entities.ProductFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.DTOs
{
    public class CustomerDto : UserDto
    {
        public ICollection<Order> Orders { get; set; }
    }
}
