using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API_WebH3.Helpers;
using API_WebH3.Models;

namespace API_WebH3.Models;

public class UserQuizAnswer
{
    [Key] public string Id { get; set; } = IdGenerator.IdUserQuizAnswer();
    [ForeignKey("User")]
    [Required]
    public Guid UserId { get; set; } 

    [ForeignKey("Quiz")]
    [Required]
    public string QuizId { get; set; } 

    [Required]
    [MaxLength(200)]
    public string UserAnswer { get; set; }

    public bool IsCorrect { get; set; } 

    [Required]
    public string AnsweredAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"); 

    [MaxLength(500)]
    public string? Feedback { get; set; } 

    public virtual User User { get; set; } 
    public virtual Quiz Quiz { get; set; } 
}