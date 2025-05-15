using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using API_WebH3.Helpers;

namespace API_WebH3.Models;

// Category for Courses
public class Category
{
    [Key]
    public string Id { get; set; } = IdGenerator.IdCategory();
    
    [Required]
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
}