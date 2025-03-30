using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(Order order);
    Task CreateOrderAsync(Order order);
    
    Task CreateOrderDetailsAsync(OrderDetail orderDetails);
    Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId);
}