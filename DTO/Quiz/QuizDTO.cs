namespace API_WebH3.Models.DTO;

public class QuizDTO
{
    public string Id { get; set; }
    public string LessonId { get; set; }
    public string Question { get; set; }
    public List<string> Options { get; set; }
    public string CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    public string CreatedAt { get; set; }
}