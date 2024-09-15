using MegStore.Application.DTOs;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using MegStore.Infrastructure.Repositories;
using System.Collections.Generic;
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

        public async Task<Order> GetOrderByIdAsync(long orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task RemoveProductFromOrderAsync(long orderId, long productId)
        {
            await _orderRepository.RemoveProductFromOrderAsync(orderId, productId);
        }

        public async Task UpdateOrderWithItemsAsync(long orderId, Order orderDto)
        {
            await _orderRepository.UpdateOrderWithItemsAsync(orderId, orderDto);
        }
     
    }
}
