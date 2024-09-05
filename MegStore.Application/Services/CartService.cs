using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
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

    }
}
