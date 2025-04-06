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
    
    public async Task<OrderDto> GetByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .Where(o => o.Id == id)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                CourseId = o.CourseId,
                Amount = o.Amount,
                Status = o.Status,
                CreatedAt = o.CreatedAt
            })
            .FirstOrDefaultAsync();
        return order;
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(OrderDto order)
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
        _context.Orders.Update(existingOrder);
        await _context.SaveChangesAsync();
    }

    public async Task CreateOrderAsync(CreateOrderDto order)
    {
        var newOrder = new Order
        {
            UserId = order.UserId,
            CourseId = order.CourseId,
            Status = "Pending",
            Amount = order.Amount
        };
        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();
    }

}