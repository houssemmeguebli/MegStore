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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                if (categories == null || !categories.Any())
                {
                    return NotFound("There are no categories.");
                }
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving categories: {ex.Message}");
            }
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(long categoryId)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return NotFound("There is no category with this ID.");
                }
                var categoryDto = _mapper.Map<CategoryDto>(category);
                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving category: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = _mapper.Map<Category>(categoryDto);
                await _categoryService.AddAsync(category);
                var createdCategoryDto = _mapper.Map<CategoryDto>(category);

                return CreatedAtAction(nameof(GetCategoryById), new { categoryId = createdCategoryDto.CategoryId }, createdCategoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding category: {ex.Message}");
            }
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(long categoryId, CategoryDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingCategory = await _categoryService.GetByIdAsync(categoryId);
                if (existingCategory == null)
                {
                    return NotFound("Category not found.");
                }

                _mapper.Map(categoryDto, existingCategory); // Map the incoming DTO to the existing category entity
                await _categoryService.UpdateAsync(existingCategory);

                return NoContent(); // 204 No Content for successful updates
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating category: {ex.Message}");
            }
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(long categoryId)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return NotFound("Category not found.");
                }

                await _categoryService.DeleteAsync(category);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting category: {ex.Message}");
            }
        }
    }
}
