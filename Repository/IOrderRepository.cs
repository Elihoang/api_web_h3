using API_WebH3.DTO.Order;
using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(string id);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(Order order);
    Task CreateOrderAsync(Order order);
    Task CreateOrderDetailsAsync(OrderDetail orderDetails);
    Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(string orderId);
    Task<IEnumerable<Order>> GetAllAsync();
}