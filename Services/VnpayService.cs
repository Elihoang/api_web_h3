
using System.Net;
using System.Security.Cryptography;
using System.Text;
using API_WebH3.DTOs.Order;
using API_WebH3.Repositories;
using API_WebH3.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Services;

public class VnpayService
{
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly EmailPaymentService _emailPaymentService;
    private readonly IEnrollementRepository _enrollementRepository;
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

    public VnpayService(
        IConfiguration configuration,
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        EmailPaymentService emailPaymentService,
        IEnrollementRepository enrollementRepository)
    {
        _configuration = configuration;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _emailPaymentService = emailPaymentService;
        _enrollementRepository = enrollementRepository;
    }

    public string CreatePaymentUrl(OrderDto orderDto, HttpContext context)
    {
        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

        _requestData.Add("vnp_Version", _configuration["Vnpay:Version"]);
        _requestData.Add("vnp_Command", _configuration["Vnpay:Command"]);
        _requestData.Add("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
        _requestData.Add("vnp_Amount", ((int)(orderDto.TotalAmount * 100)).ToString());
        _requestData.Add("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        _requestData.Add("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
        _requestData.Add("vnp_IpAddr", GetIpAddress(context));
        _requestData.Add("vnp_Locale", _configuration["Vnpay:Locale"]);
        _requestData.Add("vnp_OrderInfo", $"Thanh toán đơn hàng #{orderDto.Id}");
        _requestData.Add("vnp_OrderType", "billpayment");
        _requestData.Add("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
        _requestData.Add("vnp_TxnRef", orderDto.Id.ToString());

        var paymentUrl = CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
        return paymentUrl;
    }

public async Task<IActionResult> PaymentExecuteAsync(IQueryCollection collections)
{
    foreach (var (key, value) in collections)
    {
        if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
        {
            _responseData.Add(key, value);
        }
    }

    var orderId = Guid.Parse(GetResponseData("vnp_TxnRef"));
    var vnpResponseCode = GetResponseData("vnp_ResponseCode");
    var vnpSecureHash = collections["vnp_SecureHash"];
    var orderInfo = GetResponseData("vnp_OrderInfo");
    var checkSignature = ValidateSignature(vnpSecureHash, _configuration["Vnpay:HashSecret"]);

    var order = await _orderRepository.GetByIdAsync(orderId);
    if (order == null || !checkSignature)
    {
        return new RedirectResult("/payment-failure");
    }

    var orderDto = new OrderDto
    {
        Id = order.Id,
        UserId = order.UserId,
        TotalAmount = order.TotalAmount,
        Status = order.Status,
        CreatedAt = order.CreatedAt,
    };

    string redirectUrl;
    if (vnpResponseCode == "00")
    {
        order.Status = "Paid";
        await _orderRepository.UpdateAsync(order);
        orderDto.Status = "Paid";

        // Tự động ghi danh người dùng vào khóa học
        if (orderDto.OrderDetails != null && orderDto.OrderDetails.Any())
        {
            foreach (var detail in orderDto.OrderDetails)
            {
                var enrollment = new Enrollment
                {
                    UserId = order.UserId,
                    CourseId = detail.CourseId,
                    EnrolledAt = DateTime.UtcNow,
                    Status = "Active"
                };
                await _enrollementRepository.CreateAsync(enrollment);
            }
        }

        // Gửi email thông báo
        var user = await _userRepository.GetByIdAsync(order.UserId);
        if (user != null && !string.IsNullOrEmpty(user.Email))
        {
            var subject = "Thanh toán thành công - Đơn hàng #" + order.Id;
            var body = $@"<h2>Chúc mừng bạn đã thanh toán thành công!</h2>
                        <p>Cảm ơn bạn đã đăng ký khóa học của chúng tôi.</p>
                        <p><strong>Thông tin đơn hàng:</strong></p>
                        <ul>
                            <li>Mã đơn hàng: {order.Id}</li>
                            <li>Tổng tiền: {order.TotalAmount:N0} VND</li>
                            <li>Thời gian: {order.CreatedAt}</li>
                        </ul>
                        <p>Trân trọng,<br>H3 xin cảm ơn</p>";

            try
            {
                await _emailPaymentService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {user.Email}: {ex.Message}");
            }
        }

        // Chuẩn bị URL redirect cho trang thành công
        redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-success/{order.Id}" +
                     $"?vnp_Amount={(int)(order.TotalAmount * 100)}" +
                     $"&vnp_OrderInfo={WebUtility.UrlEncode(orderInfo)}" +
                     $"&vnp_ResponseCode={vnpResponseCode}";
    }
    else if (vnpResponseCode == "24")
    {
        order.Status = "Cancelled";
        await _orderRepository.UpdateAsync(order);
        orderDto.Status = "Cancelled";
        redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-failure";
    }
    else
    {
        order.Status = "Failed";
        await _orderRepository.UpdateAsync(order);
        orderDto.Status = "Failed";
        redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-failure";
    }

    // Log redirectUrl để kiểm tra
    Console.WriteLine("Redirect URL: " + redirectUrl);
    return new RedirectResult(redirectUrl);
}

    private string GetIpAddress(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }

    private string CreateRequestUrl(string baseUrl, string vnpHashSecret)
    {
        var data = new StringBuilder();
        foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
        }

        var querystring = data.ToString().TrimEnd('&');
        var signData = querystring;
        var vnpSecureHash = HmacSha512(vnpHashSecret, signData);
        return baseUrl + "?" + querystring + "&vnp_SecureHash=" + vnpSecureHash;
    }

    private bool ValidateSignature(string inputHash, string secretKey)
    {
        var rspRaw = GetResponseData();
        var myChecksum = HmacSha512(secretKey, rspRaw);
        return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string GetResponseData(string key = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            var data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType")) _responseData.Remove("vnp_SecureHashType");
            if (_responseData.ContainsKey("vnp_SecureHash")) _responseData.Remove("vnp_SecureHash");

            foreach (var (k, v) in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(k) + "=" + WebUtility.UrlEncode(v) + "&");
            }

            if (data.Length > 0) data.Length -= 1;
            return data.ToString();
        }
        return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
    }

    private string HmacSha512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using var hmac = new HMACSHA512(keyBytes);
        var hashValue = hmac.ComputeHash(inputBytes);
        return Convert.ToHexString(hashValue).ToLower();
    }
}

public class VnPayCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        return string.CompareOrdinal(x, y);
    }
}