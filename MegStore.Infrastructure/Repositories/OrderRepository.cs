using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using MegStore.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly MegStoreContext _context;

        public OrderRepository(MegStoreContext context) : base(context)
        {
            _context = context;
        }
    }
}
