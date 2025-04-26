using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class OrderRepository: IOrderRepository
{
    private readonly AppDbContext _context;
    
    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Order> GetByIdAsync(Guid id)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task CreateOrderDetailsAsync(OrderDetail orderDetails)
    {
        _context.OrderDetails.Add(orderDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId)
    {
        return await _context.OrderDetails
            .Where(od => od.OrderId == orderId)
            .ToListAsync();
    }
}