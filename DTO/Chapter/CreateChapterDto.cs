using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Chapter;

public class CreateChapterDto
{
    [Required]
    public Guid CourseId { get; set; }
    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
    [Required]
    public int OrderNumber { get; set; }
}