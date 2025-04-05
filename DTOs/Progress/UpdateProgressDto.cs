namespace API_WebH3.DTOs.Progress;

public class UpdateProgressDto
{
    public string? Status { get; set; }
    public int? CompletionPercentage { get; set; }
    public string? Notes { get; set; }
}