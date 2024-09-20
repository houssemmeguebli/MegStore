using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.Services
{
    public class CouponService : Service<Coupon>, ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(ICouponRepository repository) : base(repository)
        {

            _couponRepository = repository;
        }

    }
}
