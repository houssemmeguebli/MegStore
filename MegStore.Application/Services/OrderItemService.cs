using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.Services
{
    public class OrderItemService : Service<OrderItem>, IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemService(IOrderItemRepository repository) : base(repository)
        {

            _orderItemRepository = repository;
        }

    }
}
