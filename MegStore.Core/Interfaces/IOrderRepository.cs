using MegStore.Core.Entities.ProductFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetOrderByIdAsync(long orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task RemoveProductFromOrderAsync(long orderId, long productId);
        Task UpdateOrderWithItemsAsync(long orderId, Order orderDto);
        


    }
}
