using Azure.Core;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Entities.Users;
using MegStore.Core.Interfaces;
using MegStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly MegStoreContext _context;

        public UserRepository(MegStoreContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(long customerId)
        {
            var Orders = await _context.Orders
                .Where(r => r.customerId == customerId).ToListAsync();
              

            if (Orders == null || !Orders.Any())
            {
                throw new Exception("No orders found for this customer.");
            }

            return Orders;
        }
        public async Task<List<User>> GetUsersWithRole(int role)
        {
    
            Role userRole = (Role)role;
            return await _context.Users
                                 .Where(u => u.role == userRole)
                                 .ToListAsync();
        }



    }
}
