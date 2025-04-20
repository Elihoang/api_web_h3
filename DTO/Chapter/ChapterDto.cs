namespace API_WebH3.DTO.Chapter;

public class ChapterDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int OrderNumber { get; set; }
    public string CreatedAt { get; set; }
}