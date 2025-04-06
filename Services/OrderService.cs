using API_WebH3.DTOs.Order;
using API_WebH3.Models;
using API_WebH3.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEnrollementRepository _enrollmentRepository;

    public OrderService(IOrderRepository orderRepository, IEnrollementRepository enrollmentRepository)
    {
        _orderRepository = orderRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<OrderDto> CreateOrder(CreateOrderDto order)
    {
        var orderEntity = new Order
        {
            Id = order.Id,
            UserId = order.UserId,
            CourseId = order.CourseId,
            Amount = order.Amount,
            Status = order.Status ?? "Pending",
        };

        try
        {
            await _orderRepository.CreateOrderAsync(orderEntity);
            Console.WriteLine($"Order created successfully: Id={orderEntity.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating order: {ex.Message}");
            throw new Exception("Không thể tạo đơn hàng: " + ex.Message);
        }

        var createdOrder = await _orderRepository.GetByIdAsync(orderEntity.Id);
        if (createdOrder == null)
        {
            throw new Exception("Không thể tìm thấy đơn hàng vừa tạo.");
        }

        return new OrderDto
        {
            Id = createdOrder.Id,
            UserId = createdOrder.UserId,
            UserName = createdOrder.User?.FullName,
            CourseId = createdOrder.CourseId,
            CourseName = createdOrder.Course?.Title,
            Amount = createdOrder.Amount,
            Status = createdOrder.Status,
            CreatedAt = createdOrder.CreatedAt
        };
    }

    public async Task<OrderDto> GetOrderById(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {
            Console.WriteLine($"Order not found: Id={id}");
            return null;
        }

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            UserName = order.User?.FullName,
            CourseId = order.CourseId,
            CourseName = order.Course?.Title,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserId(Guid userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);

        if (orders == null || !orders.Any())
        {
            Console.WriteLine($"No orders found for UserId={userId}");
            return Enumerable.Empty<OrderDto>();
        }

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            UserName = order.User?.FullName,
            CourseId = order.CourseId,
            CourseName = order.Course?.Title,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        });
    }

    public async Task<OrderDto> UpdateOrderStatus(Guid id, string status)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            Console.WriteLine($"Order not found for status update: Id={id}");
            return null;
        }

        var orderEntity = new Order
        {
            Id = order.Id,
            UserId = order.UserId,
            CourseId = order.CourseId,
            Amount = order.Amount,
            Status = status,
            CreatedAt = order.CreatedAt
        };

        try
        {
            await _orderRepository.UpdateAsync(orderEntity);
            Console.WriteLine($"Order status updated: Id={id}, Status={status}");

            if (status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
            {
                var existingEnrollment = await _enrollmentRepository.GetByUserAndCourseAsync(order.UserId, order.CourseId);
                if (existingEnrollment == null)
                {
                    var enrollmentEntity = new Enrollment
                    {
                        UserId = order.UserId,
                        CourseId = order.CourseId,
                        Status = "Atvice", // Sử dụng "Atvice" thay vì "Enrolled"
                        EnrolledAt = DateTime.UtcNow, // Sử dụng UTC
                        CreatedAt = DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss") // Chuỗi thời gian
                    };
                    await _enrollmentRepository.CreateAsync(enrollmentEntity);
                    Console.WriteLine($"Enrollment created: UserId={order.UserId}, CourseId={order.CourseId}");
                }
                else if (existingEnrollment.Status != "Atvice") // Kiểm tra với "Atvice"
                {
                    existingEnrollment.Status = "Atvice"; // Cập nhật thành "Atvice"
                    existingEnrollment.EnrolledAt = DateTime.UtcNow; // Sử dụng UTC
                    await _enrollmentRepository.UpdateAsync(existingEnrollment);
                    Console.WriteLine($"Enrollment updated to Atvice: UserId={order.UserId}, CourseId={order.CourseId}");
                }
                else
                {
                    Console.WriteLine($"Enrollment already active: UserId={order.UserId}, CourseId={order.CourseId}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating order status or managing enrollment: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw new Exception("Không thể cập nhật trạng thái đơn hàng hoặc quản lý enrollment: " + ex.Message, ex);
        }

        var updatedOrder = await _orderRepository.GetByIdAsync(id);
        return new OrderDto
        {
            Id = updatedOrder.Id,
            UserId = updatedOrder.UserId,
            UserName = updatedOrder.User?.FullName,
            CourseId = updatedOrder.CourseId,
            CourseName = updatedOrder.Course?.Title,
            Amount = updatedOrder.Amount,
            Status = updatedOrder.Status,
            CreatedAt = updatedOrder.CreatedAt
        };
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            UserName = order.User?.FullName,
            CourseId = order.CourseId,
            CourseName = order.Course?.Title,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        });
    }

    public async Task<bool> DeleteOrderAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            Console.WriteLine($"Order not found for deletion: Id={id}");
            return false;
        }

        try
        {
            // Xóa Enrollment liên quan nếu có
            var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(order.UserId, order.CourseId);
            if (enrollment != null)
            {
                await _enrollmentRepository.DeleteAsync(enrollment.Id);
                Console.WriteLine($"Enrollment deleted: UserId={order.UserId}, CourseId={order.CourseId}");
            }

            // Xóa Order
            await _orderRepository.DeleteOrderAsync(id);
            Console.WriteLine($"Order deleted successfully: Id={id}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting order: {ex.Message}");
            throw new Exception("Không thể xóa đơn hàng: " + ex.Message);
        }
    }
}