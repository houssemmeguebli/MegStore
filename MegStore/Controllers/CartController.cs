using AutoMapper;
using MegStore.Application.DTOs;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.RateLimiting;


namespace MegStore.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        private readonly ILogger<CartController> _logger; 


        public CartController(ICartService cartService, IMapper mapper, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDto>>> GetAllCarts()
        {
            try
            {
                var carts = await _cartService.GetAllAsync();
                if (carts == null || !carts.Any())
                {
                    return NotFound("No carts available.");
                }

                var cartDtos = _mapper.Map<IEnumerable<CartDto>>(carts);
                return Ok(cartDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving carts: {ex.Message}");
            }
        }

        [HttpGet("{cartId}")]
        public async Task<ActionResult<CartDto>> GetCartById(int cartId)
        {
            try
            {
                var cart = await _cartService.GetByIdAsync(cartId);
                if (cart == null)
                {
                    return NotFound("Cart not found.");
                }

                var cartDto = _mapper.Map<CartDto>(cart);
                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving cart: {ex.Message}");
            }
        }
        [HttpGet("/customerCarts/{customerId}")]
        public async Task<ActionResult<IEnumerable<CartDto>>> GetCartsForCustomer(long customerId)
        {
            // Fetch the list of carts by customer ID
            var carts = await _cartService.GetCartByCustomerIdAsync(customerId);

            if (carts == null )
            {
                return NotFound("Carts not found for the given customer.");
            }

            // Map the list of Cart entities to a list of CartDto objects
            var cartDtos = _mapper.Map<IEnumerable<CartDto>>(carts);

            return Ok(cartDtos);
        }






        [HttpPost]
        public async Task<ActionResult<CartDto>> CreateCart(CartDto cartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cart = _mapper.Map<Cart>(cartDto);
                await _cartService.AddAsync(cart);
                var createdCartDto = _mapper.Map<CartDto>(cart);

                return CreatedAtAction(nameof(GetCartById), new { cartId = createdCartDto.CartId }, createdCartDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating cart: {ex.Message}");
            }
        }

        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCart(int cartId, CartDto cartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingCart = await _cartService.GetByIdAsync(cartId);
                if (existingCart == null)
                {
                    return NotFound("Cart not found.");
                }

                _mapper.Map(cartDto, existingCart); // Map the incoming DTO to the existing cart entity
                await _cartService.UpdateAsync(existingCart);

                return NoContent(); // 204 No Content for successful updates
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating cart: {ex.Message}");
            }
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> DeleteCart(int cartId)
        {
            try
            {
                var cart = await _cartService.GetByIdAsync(cartId);
                if (cart == null)
                {
                    return NotFound("Cart not found.");
                }

                await _cartService.DeleteAsync(cart);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting cart: {ex.Message}");
            }
        }
    }
}
