using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using MegStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegStore.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly MegStoreContext _context;

        public OrderRepository(MegStoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderByIdAsync(long orderId)
        {
            return await _context.Orders
                      .Include(o => o.OrderItems)
                       .ThenInclude(oi => oi.Product)  // Include Product details for each OrderItem
                      .FirstOrDefaultAsync(o => o.orderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task RemoveProductFromOrderAsync(long orderId, long productId)
        {
            // Fetch the order including its order items
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product) // Optional, if you need product details
                .FirstOrDefaultAsync(o => o.orderId == orderId); // Ensure property names match

            if (order == null)
                throw new KeyNotFoundException("Order not found");

            // Find the OrderItem that matches the product
            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
            if (orderItem == null)
                throw new KeyNotFoundException("Product not found in order");

            // Remove the OrderItem from the context
            _context.OrderItems.Remove(orderItem);

            // Remove the OrderItem from the order's OrderItems collection
            order.OrderItems.Remove(orderItem);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderWithItemsAsync(long orderId, Order orderDto)
        {
            // Fetch the existing order with its items
            var existingOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.orderId == orderId);

            if (existingOrder == null)
                throw new KeyNotFoundException("Order not found");

            // Update order details
            _context.Entry(existingOrder).CurrentValues.SetValues(orderDto);

            // Manage OrderItems
            var existingOrderItemIds = existingOrder.OrderItems.Select(oi => oi.OrderItemId).ToList();

            foreach (var updatedOrderItem in orderDto.OrderItems)
            {
                if (updatedOrderItem.OrderItemId == 0)
                {
                    // New OrderItem
                    if (updatedOrderItem.Quantity > 0 && updatedOrderItem.TotalPrice > 0)
                    {
                        updatedOrderItem.OrderId = orderId; // Set foreign key
                        _context.OrderItems.Add(updatedOrderItem);
                    }
                }
                else
                {
                    // Existing OrderItem
                    var existingOrderItem = existingOrder.OrderItems
                        .FirstOrDefault(oi => oi.OrderItemId == updatedOrderItem.OrderItemId);

                    if (existingOrderItem != null)
                    {
                        // Update existing OrderItem
                        existingOrderItem.Quantity = updatedOrderItem.Quantity;
                        existingOrderItem.TotalPrice = updatedOrderItem.TotalPrice;
                    }
                    else
                    {
                        throw new KeyNotFoundException($"OrderItem with ID {updatedOrderItem.OrderItemId} not found.");
                    }
                }
            }

            // Remove OrderItems that are not in the updated list
            var updatedOrderItemIds = new HashSet<long>(orderDto.OrderItems.Where(oi => oi.OrderItemId > 0).Select(oi => oi.OrderItemId));
            var itemsToRemove = existingOrder.OrderItems.Where(oi => !updatedOrderItemIds.Contains(oi.OrderItemId)).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                _context.OrderItems.Remove(itemToRemove);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }




    }
}