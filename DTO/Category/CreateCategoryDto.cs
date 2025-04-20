using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Category;

public class CreateCategoryDto
{
    [Required] 
    public string Name { get; set; } 
    public string? Description { get; set; }
}