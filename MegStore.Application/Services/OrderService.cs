using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.Services
{
    public class OrderService : Service<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository repository) : base(repository)
        {
            _orderRepository = repository;
        }
    }
}
