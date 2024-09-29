using MegStore.Core.Entities.ProductFolder;
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
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly MegStoreContext _context;

        public CartRepository(MegStoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Cart>> GetCartByCustomerIdAsync(long customerId)
        {
            return await _context.Carts
                 .Include(c => c.CartItems)
                 .ThenInclude(ci => ci.Product)
                 .Where(c => c.customerId == customerId)
                 .ToListAsync();
        }

     


    }
}
