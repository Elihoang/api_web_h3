namespace API_WebH3.DTO.Course;

public class CourseDto
{
    public String Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string? UrlImage { get; set; }
    public Guid InstructorId { get; set; }
    public string? CategoryId { get; set; }
    public string CreatedAt { get; set; }
    public List<string>? Contents { get; set; }
}