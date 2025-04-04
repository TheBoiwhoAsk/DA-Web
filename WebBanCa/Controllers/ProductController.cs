using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanCa.Models;
using WebBanCa.Repositories;

namespace WebBanCa.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductWebBanCaController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductWebBanCaController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Console.WriteLine($"Received Product: {product.Name}, Price: {product.Price}, CategoryId: {product.CategoryId}");

            await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id) return BadRequest();
            await _productRepository.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Tên sản phẩm không được để trống.");
            }

            var products = await _productRepository.SearchProductsAsync(name);
            if (products == null || !products.Any())
            {
                return NotFound("Không tìm thấy sản phẩm phù hợp.");
            }

            return Ok(products);
        }

        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetProductImages(int id)
        {
            var images = await _productRepository.GetProductImagesByProductIdAsync(id);
            if (images == null || !images.Any())
            {
                return NotFound("Không tìm thấy hình ảnh cho sản phẩm này.");
            }
            return Ok(images);
        }

        [HttpPost("{id}/images")]
        public async Task<IActionResult> AddProductImage(int id, [FromBody] ProductImage productImage)
        {
            productImage.ProductId = id;
            await _productRepository.AddProductImageAsync(productImage);
            return Ok(productImage);
        }
    }
}