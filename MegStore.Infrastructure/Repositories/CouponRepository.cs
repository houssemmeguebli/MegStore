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
    public class CouponRepository :Repository<Coupon>, ICouponRepository
    {
        private readonly MegStoreContext _context;

    public CouponRepository(MegStoreContext context) : base(context)
    {
        _context = context;
    }
}
}
