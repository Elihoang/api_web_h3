using API_WebH3.Data;
using API_WebH3.DTOs.Order;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Course)
            .SingleOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Course)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Course)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        var existingOrder = await _context.Orders.SingleOrDefaultAsync(o => o.Id == order.Id);
        if (existingOrder == null)
        {
            throw new KeyNotFoundException($"Order with ID {order.Id} not found.");
        }
        existingOrder.UserId = order.UserId;
        existingOrder.CourseId = order.CourseId;
        existingOrder.Status = order.Status;
        existingOrder.Amount = order.Amount;
        existingOrder.CreatedAt = order.CreatedAt; // Đảm bảo giữ CreatedAt
        _context.Orders.Update(existingOrder);
        await _context.SaveChangesAsync();
    }

    public async Task CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteOrderAsync(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}