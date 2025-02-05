﻿using MegStore.Core.Entities.ProductFolder;
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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly MegStoreContext _context;

        public ProductRepository(MegStoreContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(long categoryId)
        {
            return await _context.Products
                                 .Where(p => p.categoryId == categoryId)
                                 .ToListAsync();
        }
        public async Task<List<Product>> GetProductByAdminId(long adminId)
        {
            {
                return await _context.Products
                                     .Where(p => p.adminId == adminId)
                                     .ToListAsync();
            }
        }


    }
}
