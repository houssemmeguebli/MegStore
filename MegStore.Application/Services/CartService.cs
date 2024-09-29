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
    public class CartService : Service<Cart>, ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository repository) : base(repository)
        {

            _cartRepository = repository;
        }

        public async Task<List<Cart>> GetCartByCustomerIdAsync(long customerId)
        {

            return await _cartRepository.GetCartByCustomerIdAsync(customerId);
        }
      
    }
}
