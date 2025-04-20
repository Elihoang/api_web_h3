using API_WebH3.DTO.Category;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategories()
    {
        var category = await _categoryRepository.GetAllAsync();
        return category.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
        });
    }

    public async Task<CategoryDto> GetCategoryById(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return null;
        }

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,

        };
    }

    public async Task<CategoryDto> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = createCategoryDto.Name,
            Description = createCategoryDto.Description,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };
        await _categoryRepository.AddAsync(category);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,

        };
    }

    public async Task<CategoryDto> UpdateCategory(Guid id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null )
        {
            return null;
        }
        category.Name = updateCategoryDto.Name;
        category.Description = updateCategoryDto.Description;
        await _categoryRepository.UpdateAsync(category);
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
        };
    }

    public async Task<bool> DeleteCategory(Guid id)
    {
        var category =  await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }

        await _categoryRepository.DeleteAsync(id);
        return true;
    }

}