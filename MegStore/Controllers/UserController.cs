using AutoMapper;
using MegStore.Application.DTOs;
using MegStore.Core.Entities.Users;
using MegStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace MegStore.Presentation.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserById(long userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"There is no user with ID {userId}");
            }
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [Authorize(Policy = "AdminOrSuperAdmin")]
        [HttpGet("users/{role}")]
        public async Task<ActionResult<List<UserDto>>> GetUsersWithRole(int role)
        {
            var users = await _userService.GetUsersWithRole(role);
            if (users == null || !users.Any())
            {
                return NotFound($"There is no users role  {role}");
            }
            var usersDto = _mapper.Map<List<UserDto>>(users);
            return Ok(usersDto);
        }
        [Authorize(Policy = "SuperAdminOnly")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            if (users == null || !users.Any())
            {
                return NotFound("There are no users");
            }
            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(usersDto);
        }
        [Authorize]
        [HttpPut("{userId}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> UpdateUser(long userId, UserDto userDto)
        {
            var existingUser = await _userService.GetByIdAsync(userId);
            if (existingUser == null)
            {
                return NotFound("There is no user with this ID");
            }

            // Map changes from userDto to existingUser
            _mapper.Map(userDto, existingUser);

            await _userService.UpdateAsync(existingUser);
            return NoContent();
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [EnableRateLimiting("fixed")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(long userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(user);
            return NoContent();
        }

        [Authorize]
        [HttpGet("customerOrders/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomerIdAsync(long customerId)
        {
          
            var orders = await _userService.GetOrdersByCustomerIdAsync(customerId);

            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for customer with ID {customerId}");
            }

            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return Ok(orderDtos);
        }

    }
}

