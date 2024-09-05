using AutoMapper;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using MegStore.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        try
        {
            var products = await _productService.GetAllAsync();
            if (products == null || !products.Any())
            {
                return NotFound("There are no products.");
            }

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving products: {ex.Message}");
        }
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductDto>> GetProductById(long productId)
    {
        try
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound("There is no product with this ID.");
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving product: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto productDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(productDto);
            await _productService.AddAsync(product);

            // Refresh the product object to get the updated values, such as ID
            var createdProduct = await _productService.GetByIdAsync(product.productId);
            var createdProductDto = _mapper.Map<ProductDto>(createdProduct);

            return CreatedAtAction(nameof(GetProductById), new { productId = createdProductDto.ProductId }, createdProductDto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding product: {ex.Message}");
        }
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProduct(long productId, ProductDto productDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _productService.GetByIdAsync(productId);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            // Map the DTO to the existing product, ignoring the primary key
            _mapper.Map(productDto, existingProduct);

            // Save the changes
            await _productService.UpdateAsync(existingProduct);

            return NoContent(); // 204 No Content for successful updates
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating product: {ex.Message}");
        }
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(long productId)
    {
        try
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            await _productService.DeleteAsync(product);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting product: {ex.Message}");
        }
    }
}
