using AutoMapper;
using MegStore.Application.DTOs;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegStore.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper, IOrderItemService orderItemService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _orderItemService = orderItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                if (orders == null || !orders.Any())
                {
                    return NotFound("There are no orders.");
                }
                var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving orders: {ex.Message}");
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(long orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound("There is no order with this ID.");
                }
                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving order: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Map OrderDto to Order entity (including OrderItems)
                var order = _mapper.Map<Order>(orderDto);

                // Ensure that order items have the correct OrderId and calculate total price
                foreach (var item in order.OrderItems)
                {
                    item.OrderId = order.orderId; // Make sure each order item has the correct order ID
                    item.TotalPrice = item.Quantity * item.UnitPrice; // Calculate the total price for each item
                }

                // Add the order with items in a single AddAsync call
                await _orderService.AddAsync(order);

                // Map the created order back to OrderDto
                var createdOrderDto = _mapper.Map<OrderDto>(order);

                // Return the created order details
                return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrderDto.OrderId }, createdOrderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding order: {ex.Message}");
            }
        }
        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(long orderId, [FromBody] Order orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Order data is null.");
            }

            if (orderId <= 0)
            {
                return BadRequest("Invalid order ID.");
            }

            try
            {
                await _orderService.UpdateOrderWithItemsAsync(orderId, orderDto);
                return Ok("Order updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here if needed
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the order.");
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(long orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                await _orderService.DeleteAsync(order);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting order: {ex.Message}");
            }
        }

        [HttpGet("Orderproducts/{id}")]
        public async Task<IActionResult> GetOrderproductsById(long id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                // Map the Order entity to OrderDto
                var orderDto = _mapper.Map<OrderDto>(order);

                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving order products: {ex.Message}");
            }
        }

        [HttpDelete("{orderId}/products/{productId}")]
        public async Task<IActionResult> RemoveProductFromOrder(long orderId, long productId)
        {
            try
            {
                // Use the updated RemoveProductFromOrderAsync to remove the product from the order via OrderItem
                await _orderService.RemoveProductFromOrderAsync(orderId, productId);
                return NoContent(); // 204 No Content indicates successful removal
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404 Not Found if the order or product is missing
            }
            catch (Exception ex)
            {
                // Log the exception as necessary for debugging
                return StatusCode(500, "An error occurred while removing the product from the order"); // 500 Internal Server Error
            }
        }
    }
}
