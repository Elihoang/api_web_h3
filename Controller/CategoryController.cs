using API_WebH3.DTO.Category;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var category = await _categoryService.GetAllCategories();
        return Ok(category);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(string id)
    {
        var category = await _categoryService.GetCategoryById(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var category = await _categoryService.CreateCategory(createCategoryDto);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(string id, UpdateCategoryDto updateCategoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var category = await _categoryService.UpdateCategory(id, updateCategoryDto);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(string id)
    {
        var category = await _categoryService.DeleteCategory(id);
        if (!category)
        {
         return NotFound();
        }
        return NoContent();
    }
    
}