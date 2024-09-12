using AutoMapper;
using MegStore.Application.DTOs;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using MegStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            try
            {
                var orders = await _orderService.GetAllAsync();
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
                var order = await _orderService.GetByIdAsync(orderId);
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

                var order = _mapper.Map<Order>(orderDto);
                await _orderService.AddAsync(order);
                var createdOrderDto = _mapper.Map<OrderDto>(order);

                return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrderDto.OrderId }, createdOrderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding order: {ex.Message}");
            }
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(long orderId, OrderDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingOrder = await _orderService.GetByIdAsync(orderId);
                if (existingOrder == null)
                {
                    return NotFound("Order not found.");
                }

                _mapper.Map(orderDto, existingOrder); // Map the incoming DTO to the existing order entity
                await _orderService.UpdateAsync(existingOrder);

                return NoContent(); // 204 No Content for successful updates
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating order: {ex.Message}");
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(long orderId)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(orderId);
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
        // GET api/orders/{id}
        [HttpGet("Orderproducts/{id}")]
        public async Task<IActionResult> GetOrderproductsById(long id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Map the Order entity to OrderDto
            var orderDto = _mapper.Map<OrderDto>(order);

            return Ok(orderDto);
        }
        [HttpDelete("{orderId}/products/{productId}")]
        public async Task<IActionResult> RemoveProductFromOrder(long orderId, long productId)
        {
            try
            {
                await _orderService.RemoveProductFromOrderAsync(orderId, productId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpPut("{orderId}/products/{productId}")]
        public async Task<IActionResult> UpdateProductInOrder(long orderId, long productId,int newQuantity)
        {
            if (newQuantity == null ||  newQuantity < 1)
            {
                return BadRequest(new { message = "Invalid product quantity" });
            }

            try
            {
                await _orderService.UpdateProductInOrderListAsync(orderId, productId, newQuantity);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
