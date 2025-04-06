using API_WebH3.DTOs.Order;
using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface IOrderRepository
{
    Task<OrderDto> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(OrderDto order);
    Task CreateOrderAsync(CreateOrderDto order);
}