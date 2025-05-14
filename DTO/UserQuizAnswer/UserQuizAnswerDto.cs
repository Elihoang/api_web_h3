namespace API_WebH3.Models.DTO;

public class UserQuizAnswerDto
{
    public string Id { get; set; }
    public string QuizId { get; set; }
    public string UserAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public string? Feedback { get; set; }
    public string AnsweredAt { get; set; }
}