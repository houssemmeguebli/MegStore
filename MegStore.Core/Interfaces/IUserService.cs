using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Interfaces
{
    public interface IUserService : IService<User>
    {
       Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(long customerId);
    }
}
