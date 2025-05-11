using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API_WebH3.DTO.Enrollment;
using API_WebH3.DTO.Order;
using API_WebH3.Models;
using API_WebH3.Repository;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Services;

public class VnpayService
{
    private readonly IConfiguration _configuration;
    private readonly OrderService _orderService;
    private readonly EnrollmentService _enrollementService;
    private readonly EmailPaymentService _emailPaymentService;
    private readonly IUserRepository _userRepository;
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

    public VnpayService(
        IConfiguration configuration,
        OrderService orderService,
        EnrollmentService enrollementService,
        EmailPaymentService emailPaymentService,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _orderService = orderService;
        _enrollementService = enrollementService;
        _emailPaymentService = emailPaymentService;
        _userRepository = userRepository;
        ValidateConfiguration();
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_configuration["Vnpay:TmnCode"])) throw new ArgumentException("Vnpay:TmnCode is not configured.");
        if (string.IsNullOrEmpty(_configuration["Vnpay:HashSecret"])) throw new ArgumentException("Vnpay:HashSecret is not configured.");
        if (string.IsNullOrEmpty(_configuration["Vnpay:BaseUrl"])) throw new ArgumentException("Vnpay:BaseUrl is not configured.");
        if (string.IsNullOrEmpty(_configuration["Vnpay:PaymentBackReturnUrl"])) throw new ArgumentException("Vnpay:PaymentBackReturnUrl is not configured.");
        if (string.IsNullOrEmpty(_configuration["Frontend:BaseUrl"])) throw new ArgumentException("Frontend:BaseUrl is not configured.");
        Console.WriteLine("VnPay and Frontend configuration validated successfully.");
    }

    public string CreatePaymentUrl(OrderDto order, HttpContext context)
    {
        if (order.Amount < 0 || order.Id == string.Empty || order.UserId == Guid.Empty)
        {
            Console.WriteLine($"Invalid order data: Amount={order.Amount}, Id={order.Id}, UserId={order.UserId}");
            throw new ArgumentException("Dữ liệu đơn hàng không hợp lệ: Amount, Id hoặc UserId không đúng.");
        }

        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"] ?? "SE Asia Standard Time");
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

        _requestData.Clear();
        _requestData.Add("vnp_Version", _configuration["Vnpay:Version"] ?? "2.1.0");
        _requestData.Add("vnp_Command", _configuration["Vnpay:Command"] ?? "pay");
        _requestData.Add("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
        _requestData.Add("vnp_Amount", ((int)(order.Amount * 100)).ToString());
        _requestData.Add("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        _requestData.Add("vnp_CurrCode", _configuration["Vnpay:CurrCode"] ?? "VND");
        _requestData.Add("vnp_IpAddr", GetIpAddress(context));
        _requestData.Add("vnp_Locale", _configuration["Vnpay:Locale"] ?? "vn");
        _requestData.Add("vnp_OrderInfo", $"Thanh toán đơn hàng #{order.Id}");
        _requestData.Add("vnp_OrderType", "billpayment");
        _requestData.Add("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
        _requestData.Add("vnp_TxnRef", order.Id.ToString());

        Console.WriteLine("Request data for payment URL:");
        foreach (var (key, value) in _requestData)
        {
            Console.WriteLine($"{key}: {value}");
        }

        return CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
    }

    public async Task<IActionResult> PaymentExecuteAsync(IQueryCollection collections)
    {
        _responseData.Clear();
        foreach (var (key, value) in collections)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                _responseData.Add(key, value);
            }
        }

        if (!_responseData.ContainsKey("vnp_TxnRef"))
        {
            Console.WriteLine("Missing vnp_TxnRef in response data.");
            return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=MissingOrderId");
        }

        Console.WriteLine($"Received vnp_TxnRef: {GetResponseData("vnp_TxnRef")}");

        var txnRefRaw = GetResponseData("vnp_TxnRef");
        if (string.IsNullOrWhiteSpace(txnRefRaw))
        {
            Console.WriteLine($"Invalid vnp_TxnRef format: {GetResponseData("vnp_TxnRef")}");
            return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=InvalidOrderId");
        }
        var orderId = txnRefRaw;

        var vnpResponseCode = GetResponseData("vnp_ResponseCode");
        var vnpSecureHash = collections["vnp_SecureHash"];
        var orderInfo = GetResponseData("vnp_OrderInfo");

        if (string.IsNullOrEmpty(vnpSecureHash))
        {
            Console.WriteLine("Missing vnp_SecureHash in response data.");
            return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=MissingSignature");
        }

        var checkSignature = ValidateSignature(vnpSecureHash, _configuration["Vnpay:HashSecret"]);
        Console.WriteLine($"Signature validation: {checkSignature}");

        var order = await _orderService.GetOrderById(orderId);
        if (order == null)
        {
            Console.WriteLine($"Order not found: Id={orderId}");
            return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=OrderNotFound");
        }

        if (!checkSignature)
        {
            Console.WriteLine($"Invalid signature for order: Id={orderId}");
            return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=InvalidSignature");
        }

        string redirectUrl;
        switch (vnpResponseCode)
        {
            case "00":
                Console.WriteLine($"Payment successful for order: Id={orderId}");
                await _orderService.UpdateOrderStatus(orderId, "Paid");

                foreach (var detail in order.OrderDetails)
                {
                    var existingEnrollment = await _enrollementService.GetByUserAndCourseAsync(order.UserId, detail.CourseId);
                    if (existingEnrollment == null)
                    {
                        Console.WriteLine($"Creating enrollment for UserId={order.UserId}, CourseId={detail.CourseId}");
                        await _enrollementService.CreateAsync(new CreateEnrollmentDto
                        {
                            UserId = order.UserId,
                            CourseId = detail.CourseId,
                            Status = "Enrolled"
                        });
                    }
                }

                var user = await _userRepository.GetByIdAsync(order.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    Console.WriteLine($"🔹 Tìm thấy người dùng: Id={user.Id}, Email={user.Email}");
                    var subject = $"Thanh toán thành công - Đơn hàng #{order.Id}";
                    var body = $@"<h2>Chúc mừng bạn đã thanh toán thành công!</h2>
                                <p>Cảm ơn bạn đã đăng ký khóa học của chúng tôi.</p>
                                <p><strong>Thông tin đơn hàng:</strong></p>
                                <ul>
                                    <li>Mã đơn hàng: {order.Id}</li>
                                    <li>Tổng tiền: {order.Amount:N0} VND</li>
                                    <li>Thời gian: {order.CreatedAt}</li>
                                </ul>
                                <p><strong>Chi tiết đơn hàng:</strong></p>
                                <ul>
                                    {string.Join("", order.OrderDetails.Select(d => $"<li>Khóa học ID: {d.CourseId}, Giá: {d.Price:N0} VND, Giảm giá: {(d.DiscountAmount ?? 0):N0} VND</li>"))}
                                </ul>
                                <p>Trân trọng,<br>H3 xin cảm ơn</p>";
                    try
                    {
                        await _emailPaymentService.SendEmailAsync(user.Email, subject, body);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Lỗi khi gửi email thông báo: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ Email của người dùng trống hoặc không tìm thấy: UserId={order.UserId}");
                }

                redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-success/{orderId}" +
                             $"?vnp_Amount={GetResponseData("vnp_Amount")}" +
                             $"&vnp_OrderInfo={WebUtility.UrlEncode(orderInfo)}" +
                             $"&vnp_ResponseCode={vnpResponseCode}";
                break;
            case "24":
                Console.WriteLine($"Payment cancelled for order: Id={orderId}");
                await _orderService.UpdateOrderStatus(orderId, "Cancelled");
                redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-failure?reason=cancelled";
                break;
            default:
                Console.WriteLine($"Payment failed for order: Id={orderId}, ResponseCode={vnpResponseCode}");
                await _orderService.UpdateOrderStatus(orderId, "Failed");
                redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-failure?reason=failed&vnp_ResponseCode={vnpResponseCode}";
                break;
        }

        Console.WriteLine("Redirect URL: " + redirectUrl);
        return new RedirectResult(redirectUrl);
    }

    private string GetIpAddress(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        Console.WriteLine($"Client IP: {ip}");
        return ip;
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
        Console.WriteLine($"Generated vnp_SecureHash: {vnpSecureHash}");
        return baseUrl + "?" + querystring + "&vnp_SecureHash=" + vnpSecureHash;
    }

    private bool ValidateSignature(string inputHash, string secretKey)
    {
        var rspRaw = GetResponseData();
        Console.WriteLine($"Data for signature validation: {rspRaw}");
        var myChecksum = HmacSha512(secretKey, rspRaw);
        Console.WriteLine($"Calculated checksum: {myChecksum}, Input hash: {inputHash}");
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