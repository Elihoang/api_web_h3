using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using API_WebH3.Helpers;

namespace API_WebH3.Models;

// Quiz for multiple-choice questions
public class Quiz
{
    [Key] 
    public string Id { get; set; } = IdGenerator.IdQuiz();
    
    [ForeignKey("Lesson")]
    public string LessonId { get; set; }
    
    [Required]
    public required string Question { get; set; }
    
    [Required]
    [NotMapped]
    public required List<string> Options { get; set; }
    
    public string? SerializedOptions
    {
        get => Options == null ? null : JsonSerializer.Serialize(Options);
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Options = null;
            }
            else
            {
                try
                {
                    Options = JsonSerializer.Deserialize<List<string>>(value);
                }
                catch
                {
                    Options = null;
                }
            }
        }
    }
    
    [Required]
    public required string CorrectAnswer { get; set; }
    
    public string? Explanation { get; set; }
    
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual Lesson Lesson { get; set; }
}