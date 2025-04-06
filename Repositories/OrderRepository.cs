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
        existingOrder.Status = order.Status; // Ch? c?p nh?t tr?ng thái
        _context.Orders.Update(existingOrder);
        await _context.SaveChangesAsync();
    }

    public async Task CreateOrderAsync(CreateOrderDto order)
    {
        var newOrder = new Order
        {
            Id = order.Id, // S? d?ng Id t? CreateOrderDto
            UserId = order.UserId,
            CourseId = order.CourseId,
            Amount = order.Amount,
            Status = "Pending",
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };
        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();
    }
}