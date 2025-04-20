using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Course;

public class CreateCourseDto
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    public string? UrlImage { get; set; }
    [Required]
    public Guid InstructorId { get; set; }
    public Guid? CategoryId { get; set; }
    public List<string>? Contents { get; set; }
}