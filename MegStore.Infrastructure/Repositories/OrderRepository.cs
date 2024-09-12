using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using MegStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                .Include(o => o.Products) // Include products for each order
                .FirstOrDefaultAsync(o => o.orderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Products) // Include products for each order
                .ToListAsync();
        }
        // Remove a product from an order
        public async Task RemoveProductFromOrderAsync(long orderId, long productId)
        {
            // Fetch the order including its products
            var order = await _context.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.orderId == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found");

            // Find the product in the order
            var orderProduct = order.Products.FirstOrDefault(op => op.productId == productId);
            if (orderProduct == null)
                throw new KeyNotFoundException("Product not found in order");

            // Remove the product from the order's product list
            order.Products.Remove(orderProduct);

            _context.Orders.Update(order); // Update the order in the context
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        // Update the quantity or details of a product in the order
        public async Task UpdateProductInOrderListAsync(long orderId, long productId, int newQuantity)
        {
            // Fetch the order including its products
            var order = await _context.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.orderId == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found");

            // Find the product in the order
            var orderProduct = order.Products.FirstOrDefault(op => op.productId == productId);
            if (orderProduct == null)
                throw new KeyNotFoundException("Product not found in order");

            // Update the product quantity
            orderProduct.ItemQuantiy = newQuantity;

            _context.Orders.Update(order); // Update the order in the context
            await _context.SaveChangesAsync(); // Save changes to the database
        }

    }
}
