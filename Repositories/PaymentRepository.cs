using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.User)
            .Include(p => p.Course)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments
            .Include(p => p.User)
            .Include(p => p.Course)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<Payment?> UpdateAsync(Payment payment)
    {
        var existingPayment = await _context.Payments.FindAsync(payment.Id);
        if (existingPayment == null)
        {
            return null;
        }

        _context.Entry(existingPayment).CurrentValues.SetValues(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
        {
            return false;
        }

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Payment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Payments
            .Where(p => p.UserId == userId)
            .Include(p => p.User)
            .Include(p => p.Course)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Payments
            .Where(p => p.CourseId == courseId)
            .Include(p => p.User)
            .Include(p => p.Course)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetByStatusAsync(string status)
    {
        return await _context.Payments
            .Where(p => p.Status == status)
            .Include(p => p.User)
            .Include(p => p.Course)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
} 