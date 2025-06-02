namespace API_WebH3.Models.DTO;

public class CreateQuizDto
{
    public string LessonId { get; set; }
    public string Question { get; set; }
    public List<string> Options { get; set; }
    public string CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    
}