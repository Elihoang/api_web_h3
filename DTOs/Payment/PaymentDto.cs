namespace API_WebH3.DTOs.Payment;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string Status { get; set; }
    public string CreatedAt { get; set; }
}