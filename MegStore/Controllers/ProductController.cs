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
    public async Task<ActionResult<ProductDto>> CreateProduct([FromForm] ProductDto productDto, IFormFile imageFile)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string imageUrl = null;

            // If an image file is uploaded, save it
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "products");

                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique filename for the uploaded file
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Generate the URL for the uploaded image (assuming you serve images from the 'wwwroot' folder)
                imageUrl = Path.Combine("uploads", "products", uniqueFileName).Replace("\\", "/");
            }

            // Map the product DTO to the Product entity
            var product = _mapper.Map<Product>(productDto);

            // Set the imageUrl in the product if it was uploaded
            if (!string.IsNullOrEmpty(imageUrl))
            {
                product.ImageUrl = imageUrl;
            }

            // Save the product to the database
            await _productService.AddAsync(product);

            // Refresh the product object to get the updated values (such as the ID)
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
