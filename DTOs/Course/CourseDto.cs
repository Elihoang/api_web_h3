namespace API_WebH3.DTOs.Course;

public class CourseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public decimal Price { get; set; }
    
    public Guid InstructorId { get; set; }

    public string? UrlImage { get; set; }
    
    public string CreatedAt { get; set; }
    public List<string>? Contents { get; set; }

}