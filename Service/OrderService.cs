using API_WebH3.Models;
using API_WebH3.Repository;
using API_WebH3.DTO.Order;
using API_WebH3.Helpers;

namespace API_WebH3.Service;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICourseRepository _courseRepository;

    public OrderService(IOrderRepository orderRepository, ICourseRepository courseRepository)
    {
        _orderRepository = orderRepository;
        _courseRepository = courseRepository;
    }

    public async Task<OrderDto> CreateOrderWithDetailsAsync(CreateOrderDto orderDto)
    {
        // Kiểm tra dữ liệu đầu vào
        if (orderDto.OrderDetails.Any(d => string.IsNullOrEmpty(d.CourseId)))
            throw new ArgumentException("CourseId không được để trống trong OrderDetails.");

        // Kiểm tra xem CourseId có tồn tại
        foreach (var detailDto in orderDto.OrderDetails)
        {
            if (!await _courseRepository.ExistsAsync(detailDto.CourseId))
                throw new ArgumentException($"CourseId {detailDto.CourseId} không tồn tại.");
        }

        var order = new Order
        {
            Id = IdGenerator.IdOrder(),
            UserId = orderDto.UserId,
            Amount = orderDto.Amount,
            Status = orderDto.Status,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _orderRepository.CreateOrderAsync(order);

        foreach (var detailDto in orderDto.OrderDetails)
        {
            var orderDetail = new OrderDetail
            {
                Id = IdGenerator.IdOrderDetail(),
                OrderId = order.Id,
                CourseId = detailDto.CourseId,
                Price = detailDto.Price,
                CouponId = detailDto.CouponId,
                DiscountAmount = detailDto.DiscountAmount,
                CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
            };
            await _orderRepository.CreateOrderDetailsAsync(orderDetail);
        }

        return new OrderDto
        {
            Id = order.Id,
            UserId = orderDto.UserId,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            OrderDetails = orderDto.OrderDetails.Select(d => new OrderDetail
            {
                CourseId = d.CourseId,
                Price = d.Price,
                CouponId = d.CouponId,
                DiscountAmount = d.DiscountAmount
            }).ToList()
        };
    }

    public async Task<OrderDto> GetOrderById(string id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return null;

        var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(id);
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            User = order.User,
            OrderDetails = orderDetails.ToList()
        };
    }

    public async Task UpdateOrderStatus(string id, string status)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) throw new ArgumentException("Đơn hàng không tồn tại.");
        order.Status = status;
        await _orderRepository.UpdateAsync(order);
    }

    public async Task<(List<OrderDto> Orders, int TotalItems)> GetAllOrdersAsync(int pageNumber, int pageSize)
    {
        var allOrders = await _orderRepository.GetAllAsync();
        var totalItems = allOrders.Count();

        var pagedOrders = allOrders
            .OrderByDescending(o => o.CreatedAt) // nếu cần
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var orderDtos = new List<OrderDto>();

        foreach (var order in pagedOrders)
        {
            var details = await _orderRepository.GetOrderDetailsByOrderIdAsync(order.Id);

            var orderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                Amount = order.Amount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                User = order.User,
                OrderDetails = details.ToList()
            };

            orderDtos.Add(orderDto);
        }

        return (orderDtos, totalItems);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(Guid userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        var orderDtos = new List<OrderDto>();

        foreach (var order in orders)
        {
            var orderDto = await GetOrderById(order.Id);
            if (orderDto != null)
            {
                orderDtos.Add(orderDto);
            }
        }

        return orderDtos;
    }

    public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(string orderId)
    {
        var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(orderId);
        return orderDetails;
    }
}