using AutoMapper;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using MegStore.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MegStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;



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

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategoryId(long categoryId)
    {
        var products = await _productService.GetProductsByCategoryIdAsync(categoryId);

        if (products == null)
        {
            return NotFound();
        }

        // Map the products to ProductDto
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(productDtos);
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
    public async Task<ActionResult<ProductDto>> CreateProduct([FromForm] ProductDto productDto, List<IFormFile> imageFiles)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageUrls = new List<string>();

            // Process each uploaded image file
            foreach (var imageFile in imageFiles)
            {
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
                    var imageUrl = Path.Combine("uploads", "products", uniqueFileName).Replace("\\", "/");
                    imageUrls.Add(imageUrl);
                }
            }

            // Map the product DTO to the Product entity
            var product = _mapper.Map<Product>(productDto);

            // Set the imageUrls in the product if images were uploaded
            if (imageUrls.Any())
            {
                product.ImageUrls = imageUrls;
            }

            // Save the product to the database
            await _productService.AddAsync(product);

            // Refresh the product object to get the updated values (such as the ID)
            var createdProduct = await _productService.GetByIdAsync(product.productId);
            var createdProductDto = _mapper.Map<ProductDto>(createdProduct);

            return CreatedAtAction(nameof(GetProductById), new { productId = createdProductDto.productId }, createdProductDto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding product: {ex.Message}");
        }
    }
    [HttpPut("{productId}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(long productId, [FromForm] ProductDto productDto, List<IFormFile> imageFiles)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Fetch the existing product from the database
            var existingProduct = await _productService.GetByIdAsync(productId);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {productId} not found.");
            }

            var imageUrls = new List<string>(existingProduct.ImageUrls ?? new List<string>());

            // Process each uploaded image file
            foreach (var imageFile in imageFiles)
            {
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

                    // Generate the URL for the uploaded image
                    var imageUrl = Path.Combine("uploads", "products", uniqueFileName).Replace("\\", "/");
                    imageUrls.Add(imageUrl);
                }
            }

            // Map the DTO to the existing product entity (partial update)
            _mapper.Map(productDto, existingProduct);

            // Update the imageUrls in the product if images were uploaded
            if (imageUrls.Any())
            {
                existingProduct.ImageUrls = imageUrls;
            }

            // Update the product in the database
            await _productService.UpdateAsync(existingProduct);

            // Fetch the updated product from the database
            var updatedProduct = await _productService.GetByIdAsync(productId);
            var updatedProductDto = _mapper.Map<ProductDto>(updatedProduct);

            return Ok(updatedProductDto);
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

        [HttpDelete("{productId}/delete-image")]
        public async Task<ActionResult<ProductDto>> DeleteImage(long productId, [FromQuery] string imageUrlToDelete)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrlToDelete))
                {
                    return BadRequest("No image URL provided for deletion.");
                }

                // Fetch the existing product from the database
                var existingProduct = await _productService.GetByIdAsync(productId);
                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {productId} not found.");
                }

                var existingImageUrls = existingProduct.ImageUrls ?? new List<string>();

                // Check if the image URL exists in the list
                if (!existingImageUrls.Contains(imageUrlToDelete))
                {
                    return NotFound("Image URL not found in product.");
                }

                // Remove the image URL from the product's image URLs list
                existingImageUrls.Remove(imageUrlToDelete);

                // Delete the file from the server
                var fileName = Path.GetFileName(imageUrlToDelete);
                var filePath = Path.Combine("wwwroot", "uploads", "products", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Update the product with the remaining images
                existingProduct.ImageUrls = existingImageUrls;

                // Update the product in the database
                await _productService.UpdateAsync(existingProduct);

                // Map to DTO and return updated product
                var updatedProductDto = _mapper.Map<ProductDto>(existingProduct);
                return Ok(updatedProductDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting image: {ex.Message}");
            }
        }

}
