using MailKit.Search;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using MegStore.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.Services
{
    public class ProductService : Service<Product>, IProductService
    {

        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository repository) : base(repository)
        {
            _productRepository = repository;
        }


        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(long categoryId)
        {
            return await _productRepository.GetProductsByCategoryIdAsync(categoryId);
        }
        public async Task<List<Product>> GetProductByAdminId(long adminId)
        {
            return await _productRepository.GetProductByAdminId(adminId);
        }
    }

}