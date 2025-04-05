using API_WebH3.DTOs.Payment;
using API_WebH3.Models;
using API_WebH3.Repositories;

namespace API_WebH3.Services;

public class PaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<List<PaymentDto>> GetAllAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();
        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            UserId = p.UserId,
            UserName = p.User.FullName,
            CourseId = p.CourseId,
            CourseName = p.Course.Title,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        }).ToList();
    }

    public async Task<PaymentDto?> GetByIdAsync(Guid id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
        {
            return null;
        }

        return new PaymentDto
        {
            Id = payment.Id,
            UserId = payment.UserId,
            UserName = payment.User.FullName,
            CourseId = payment.CourseId,
            CourseName = payment.Course.Title,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto createPaymentDto)
    {
        var payment = new Payment
        {
            UserId = createPaymentDto.UserId,
            CourseId = createPaymentDto.CourseId,
            Amount = createPaymentDto.Amount,
            PaymentMethod = createPaymentDto.PaymentMethod,
            Status = createPaymentDto.Status,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        var createdPayment = await _paymentRepository.CreateAsync(payment);
        return new PaymentDto
        {
            Id = createdPayment.Id,
            UserId = createdPayment.UserId,
            UserName = createdPayment.User.FullName,
            CourseId = createdPayment.CourseId,
            CourseName = createdPayment.Course.Title,
            Amount = createdPayment.Amount,
            PaymentMethod = createdPayment.PaymentMethod,
            Status = createdPayment.Status,
            CreatedAt = createdPayment.CreatedAt
        };
    }

    public async Task<PaymentDto?> UpdateAsync(Guid id, UpdatePaymentDto updatePaymentDto)
    {
        var existingPayment = await _paymentRepository.GetByIdAsync(id);
        if (existingPayment == null)
        {
            return null;
        }

        if (updatePaymentDto.Status != null)
            existingPayment.Status = updatePaymentDto.Status;
        if (updatePaymentDto.PaymentMethod != null)
            existingPayment.PaymentMethod = updatePaymentDto.PaymentMethod;

        var updatedPayment = await _paymentRepository.UpdateAsync(existingPayment);
        if (updatedPayment == null)
        {
            return null;
        }

        return new PaymentDto
        {
            Id = updatedPayment.Id,
            UserId = updatedPayment.UserId,
            UserName = updatedPayment.User.FullName,
            CourseId = updatedPayment.CourseId,
            CourseName = updatedPayment.Course.Title,
            Amount = updatedPayment.Amount,
            PaymentMethod = updatedPayment.PaymentMethod,
            Status = updatedPayment.Status,
            CreatedAt = updatedPayment.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _paymentRepository.DeleteAsync(id);
    }

    public async Task<List<PaymentDto>> GetByUserIdAsync(Guid userId)
    {
        var payments = await _paymentRepository.GetByUserIdAsync(userId);
        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            UserId = p.UserId,
            UserName = p.User.FullName,
            CourseId = p.CourseId,
            CourseName = p.Course.Title,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        }).ToList();
    }

    public async Task<List<PaymentDto>> GetByCourseIdAsync(Guid courseId)
    {
        var payments = await _paymentRepository.GetByCourseIdAsync(courseId);
        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            UserId = p.UserId,
            UserName = p.User.FullName,
            CourseId = p.CourseId,
            CourseName = p.Course.Title,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        }).ToList();
    }

    public async Task<List<PaymentDto>> GetByStatusAsync(string status)
    {
        var payments = await _paymentRepository.GetByStatusAsync(status);
        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            UserId = p.UserId,
            UserName = p.User.FullName,
            CourseId = p.CourseId,
            CourseName = p.Course.Title,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        }).ToList();
    }
} 