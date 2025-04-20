using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Category;

public class UpdateCategoryDto
{
    [Required] 
    public string Name { get; set; } 
    public string? Description { get; set; }
}