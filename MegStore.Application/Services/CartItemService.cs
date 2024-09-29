using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.Services
{
    public class CartItemService : Service<CartItem>, ICartItemService
    {

        private readonly ICartItemRepository _cartItemRepository;

        public CartItemService(ICartItemRepository repository) : base(repository)
        {
            _cartItemRepository = repository;
        }

    }
}

