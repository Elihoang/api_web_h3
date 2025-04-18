using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace API_WebH3.Models;

// Course for online learning
public class Course
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public string? UrlImage { get; set; }

    [Required]
    [ForeignKey("User")]
    public required Guid InstructorId { get; set; }

    [ForeignKey("Category")]
    public Guid? CategoryId { get; set; }

    [Required]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    public virtual required User User { get; set; }
    public virtual Category? Category { get; set; }
    
    [NotMapped]
    public List<string>? Contents { get; set; }

    public string? SerializedContents
    {
        get => Contents == null ? null : JsonSerializer.Serialize(Contents);
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Contents = null;
            }
            else
            {
                try
                {
                    Contents = JsonSerializer.Deserialize<List<string>>(value);
                }
                catch
                {
                    Contents = null;
                }
            }
        }
    }
}