using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task UpdateProductInOrderListAsync(long orderId, long productId, int newQuantity)
        {
            await _orderRepository.UpdateProductInOrderListAsync(orderId, productId, newQuantity);
        }
    }
}
