using API_WebH3.Models;
using API_WebH3.Repository;
using API_WebH3.DTO.Order;
using API_WebH3.Helpers;

namespace API_WebH3.Service;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public OrderService(
        IOrderRepository orderRepository, 
        ICourseRepository courseRepository, 
        ICouponRepository couponRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _orderRepository = orderRepository;
        _courseRepository = courseRepository;
        _couponRepository = couponRepository;
        _enrollmentRepository = enrollmentRepository;
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

            // Kiểm tra và áp dụng coupon nếu có
            if (detailDto.CouponId.HasValue)
            {
                var coupon = await _couponRepository.GetByIdAsync(detailDto.CouponId.Value);
                if (coupon == null)
                    throw new ArgumentException($"CouponId {detailDto.CouponId} không tồn tại.");

                if (!coupon.IsActive || DateTime.UtcNow < coupon.StartDate || DateTime.UtcNow > coupon.EndDate)
                    throw new ArgumentException("Mã coupon không hợp lệ hoặc đã hết hạn.");

                if (coupon.CurrentUsage >= coupon.MaxUsage)
                    throw new ArgumentException("Mã coupon đã được sử dụng hết lượt.");

                // Kiểm tra số tiền giảm giá
                var expectedDiscount = detailDto.Price * coupon.DiscountPercentage / 100;
                if (detailDto.DiscountAmount != expectedDiscount)
                    throw new ArgumentException("Số tiền giảm giá không khớp với phần trăm giảm của coupon.");
            }
        }

        // Tính tổng số tiền đơn hàng
        decimal totalAmount = orderDto.OrderDetails.Sum(d => d.Price - (d.DiscountAmount ?? 0));
        if (orderDto.Amount != totalAmount)
            throw new ArgumentException("Tổng số tiền đơn hàng không khớp với chi tiết đơn hàng.");

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

            // Cập nhật CurrentUsage của coupon
            if (detailDto.CouponId.HasValue)
            {
                var coupon = await _couponRepository.GetByIdAsync(detailDto.CouponId.Value);
                coupon.CurrentUsage += 1;
                await _couponRepository.UpdateAsync(coupon);
            }
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

        var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(id);
        foreach (var detail in orderDetails)
        {
            if (status == "Cancelled")
            {
                // Xóa enrollment khi trạng thái là Cancelled (giữ nguyên logic hiện tại)
                await _enrollmentRepository.DeleteEnrollmentAsync(order.UserId, detail.CourseId);
            }
            else if (status == "Failed" || status == "Pending")
            {
                var enrollment = await _enrollmentRepository.GetEnrollmentAsync(order.UserId, detail.CourseId);
                if (enrollment == null)
                {
                    Console.WriteLine($"Enrollment không tìm thấy cho UserId: {order.UserId}, CourseId: {detail.CourseId}");
                    // Hoặc ghi log vào hệ thống logging của bạn
                    continue; // Bỏ qua nếu không tìm thấy enrollment
                }
                await _enrollmentRepository.UpdateEnrollmentStatusAsync(order.UserId, detail.CourseId, "Failed");
            }
        }
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