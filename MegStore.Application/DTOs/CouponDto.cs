using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.DTOs
{
    public class CouponDto
    {
        public long couponId { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int? TimesUsed { get; set; }
    }
}
