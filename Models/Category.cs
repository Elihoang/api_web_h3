using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace API_WebH3.Models;

// Category for Courses
public class Category
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
}