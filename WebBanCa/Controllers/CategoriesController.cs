using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBanCa.Models;
using WebBanCa.Repositories;

namespace WebBanCa.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryWebBanCaController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryWebBanCaController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (category == null || string.IsNullOrEmpty(category.Name))
            {
                return BadRequest("Tên danh mục không được để trống.");
            }

            await _categoryRepository.AddAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.Id) return BadRequest();

            await _categoryRepository.UpdateAsync(category);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
