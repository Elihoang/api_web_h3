namespace API_WebH3.DTOs.Payment;

public class CreatePaymentDto
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string Status { get; set; } = "pending";
} 