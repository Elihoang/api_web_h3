using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using API_WebH3.Helpers;

namespace API_WebH3.Models;

// Modified Lesson to include Chapter and multiple videos
public class Lesson
{
    [Key] 
    public string Id { get; set; } = IdGenerator.IdLesson();
    
    [ForeignKey("Chapter")]
    public Guid ChapterId { get; set; }
    
    [ForeignKey("Course")]
    public string CourseId { get; set; }
    
    [Required]
    public required string Title { get; set; }
    
    [Required]
    public required string Description { get; set; }
    
    public string? Content { get; set; }
    
    [NotMapped]
    public List<string>? VideoUrls { get; set; }
    
    public string? SerializedVideoUrls
    {
        get => VideoUrls == null ? null : JsonSerializer.Serialize(VideoUrls);
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                VideoUrls = null;
            }
            else
            {
                try
                {
                    VideoUrls = JsonSerializer.Deserialize<List<string>>(value);
                }
                catch
                {
                    VideoUrls = null;
                }
            }
        }
    }

        public int Duration { get; set; }
    
    public int OrderNumber { get; set; }
    
    [Required]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    
    [ForeignKey("ApprovedByUser")]
    public Guid? ApprovedBy { get; set; } // Admin who approved/rejected the lesson
    
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual Course Course { get; set; }
    public virtual Chapter Chapter { get; set; }
    public virtual User? ApprovedByUser { get; set; }
}