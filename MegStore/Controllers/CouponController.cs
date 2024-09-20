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

namespace MegStore.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;

        public CouponController(ICouponService couponService, IMapper mapper)
        {
            _couponService = couponService;
            _mapper = mapper;
        }

        // GET: api/coupon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetAll()
        {
            try
            {
                var coupons = await _couponService.GetAllAsync();
                if (coupons == null || !coupons.Any())
                {
                    return NotFound("There are no coupons.");
                }
                var couponDtos = _mapper.Map<IEnumerable<CouponDto>>(coupons);
                return Ok(couponDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving coupons: {ex.Message}");
            }
        }

        // GET: api/coupon/{couponId}
        [HttpGet("{couponId}")]
        public async Task<ActionResult<CouponDto>> GetCouponById(long couponId)
        {
            try
            {
                var coupon = await _couponService.GetByIdAsync(couponId);
                if (coupon == null)
                {
                    return NotFound("There is no coupon with this ID.");
                }
                var couponDto = _mapper.Map<CouponDto>(coupon);
                return Ok(couponDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving coupon: {ex.Message}");
            }
        }

        // POST: api/coupon
        [HttpPost]
        public async Task<ActionResult<CouponDto>> CreateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var coupon = _mapper.Map<Coupon>(couponDto);
                await _couponService.AddAsync(coupon);
                var createdCouponDto = _mapper.Map<CouponDto>(coupon);

                return CreatedAtAction(nameof(GetCouponById), new { couponId = createdCouponDto.couponId }, createdCouponDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding coupon: {ex.Message}");
            }
        }

        // PUT: api/coupon/{couponId}
        [HttpPut("{couponId}")]
        public async Task<IActionResult> UpdateCoupon(long couponId, [FromBody] CouponDto couponDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingCoupon = await _couponService.GetByIdAsync(couponId);
                if (existingCoupon == null)
                {
                    return NotFound("Coupon not found.");
                }

                _mapper.Map(couponDto, existingCoupon); // Map the incoming DTO to the existing coupon entity
                await _couponService.UpdateAsync(existingCoupon);

                return NoContent(); // 204 No Content for successful updates
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating coupon: {ex.Message}");
            }
        }

        // DELETE: api/coupon/{couponId}
        [HttpDelete("{couponId}")]
        public async Task<IActionResult> DeleteCoupon(long couponId)
        {
            try
            {
                var coupon = await _couponService.GetByIdAsync(couponId);
                if (coupon == null)
                {
                    return NotFound("Coupon not found.");
                }

                await _couponService.DeleteAsync(coupon);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting coupon: {ex.Message}");
            }
        }
    }
}
