using System.Net;
using System.Security.Cryptography;
using System.Text;
using API_WebH3.DTO.Order;
using API_WebH3.Models;
using API_WebH3.Repository;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API_WebH3.Services;

public class MomoService
{
    private readonly IConfiguration _configuration;
    private readonly OrderService _orderService;
    private readonly EnrollmentService _enrollmentService;
    private readonly EmailPaymentService _emailPaymentService;
    private readonly IUserRepository _userRepository;
    private readonly ICouponRepository _couponRepository;

    public MomoService(
        IConfiguration configuration,
        OrderService orderService,
        EnrollmentService enrollmentService,
        EmailPaymentService emailPaymentService,
        ICouponRepository couponRepository,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _orderService = orderService;
        _enrollmentService = enrollmentService;
        _emailPaymentService = emailPaymentService;
        _couponRepository = couponRepository;
        _userRepository = userRepository;
        ValidateConfiguration();
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_configuration["Momo:PartnerCode"])) throw new ArgumentException("Momo:PartnerCode not configured.");
        if (string.IsNullOrEmpty(_configuration["Momo:AccessKey"])) throw new ArgumentException("Momo:AccessKey not configured.");
        if (string.IsNullOrEmpty(_configuration["Momo:SecretKey"])) throw new ArgumentException("Momo:SecretKey not configured.");
        if (string.IsNullOrEmpty(_configuration["Momo:Endpoint"])) throw new ArgumentException("Momo:Endpoint not configured.");
        if (string.IsNullOrEmpty(_configuration["Momo:ReturnUrl"])) throw new ArgumentException("Momo:ReturnUrl not configured.");
        if (string.IsNullOrEmpty(_configuration["Momo:NotifyUrl"])) throw new ArgumentException("Momo:NotifyUrl not configured.");
        if (string.IsNullOrEmpty(_configuration["Frontend:BaseUrl"])) throw new ArgumentException("Frontend:BaseUrl not configured.");
    }

    public async Task<string> CreatePaymentUrlAsync(OrderDto order)
    {
        var endpoint = _configuration["Momo:Endpoint"];
        var partnerCode = _configuration["Momo:PartnerCode"];
        var accessKey = _configuration["Momo:AccessKey"];
        var secretKey = _configuration["Momo:SecretKey"];
        var returnUrl = _configuration["Momo:ReturnUrl"];
        var notifyUrl = _configuration["Momo:NotifyUrl"];
        var orderInfo = $"Thanh toán đơn hàng #{order.Id}";

        string requestId = Guid.NewGuid().ToString();
        string orderId = order.Id;
        string amount = ((int)order.Amount).ToString();

        string rawHash = $"accessKey={accessKey}&amount={amount}&extraData=&ipnUrl={notifyUrl}&orderId={orderId}" +
                         $"&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}" +
                         $"&requestId={requestId}&requestType=captureWallet";

        string signature = HmacSha256(secretKey, rawHash);

        var requestBody = new
        {
            partnerCode,
            accessKey,
            requestId,
            amount,
            orderId,
            orderInfo,
            redirectUrl = returnUrl,
            ipnUrl = notifyUrl,
            requestType = "captureWallet",
            extraData = "",
            signature
        };

        using var http = new HttpClient();
        var response = await http.PostAsync(endpoint, new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));
        var responseContent = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseContent);

        var payUrl = json["payUrl"].ToString();
        if (string.IsNullOrEmpty(payUrl))
        {
            Console.WriteLine($"Momo payment failed: {responseContent}");
            throw new Exception("Không thể tạo URL thanh toán Momo.");
        }

        return payUrl;
    }

    public async Task<IActionResult> PaymentCallbackAsync(IQueryCollection query)
    {
        try
        {
            var orderId = query["orderId"];
            var resultCode = query["resultCode"];
            var signature = query["signature"];

            var rawData = $"accessKey={_configuration["Momo:AccessKey"]}" +
                          $"&amount={query["amount"]}" +
                          $"&extraData={query["extraData"]}" +
                          $"&message={query["message"]}" +
                          $"&orderId={orderId}" +
                          $"&orderInfo={query["orderInfo"]}" +
                          $"&orderType={query["orderType"]}" +
                          $"&partnerCode={query["partnerCode"]}" +
                          $"&payType={query["payType"]}" +
                          $"&requestId={query["requestId"]}" +
                          $"&responseTime={query["responseTime"]}" +
                          $"&resultCode={resultCode}" +
                          $"&transId={query["transId"]}";

            var generatedSignature = HmacSha256(_configuration["Momo:SecretKey"], rawData);

            if (!string.Equals(signature, generatedSignature, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Momo signature mismatch");
                return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=InvalidSignature");
            }

            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
            {
                Console.WriteLine($"Order not found: {orderId}");
                return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=OrderNotFound");
            }

            if (resultCode == "0")
            {
                await _orderService.UpdateOrderStatus(orderId, "Paid");

                foreach (var detail in order.OrderDetails)
                {
                    var existingEnrollment = await _enrollmentService.GetByUserAndCourseAsync(order.UserId, detail.CourseId);
                    if (existingEnrollment == null)
                    {
                        await _enrollmentService.CreateAsync(new DTO.Enrollment.CreateEnrollmentDto
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
                    var subject = $"Thanh toán thành công - Đơn hàng #{order.Id}";
                    var body = $@"<h2>Chúc mừng bạn đã thanh toán thành công!</h2>
                                <p>Cảm ơn bạn đã đăng ký khóa học của chúng tôi.</p>
                                <ul>
                                    <li>Mã đơn hàng: {order.Id}</li>
                                    <li>Tổng tiền: {order.Amount:N0} VND</li>
                                </ul>";
                    await _emailPaymentService.SendEmailAsync(user.Email, subject, body);
                }

                return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-success/{orderId}");
            }

            await _orderService.UpdateOrderStatus(orderId, "Failed");
            return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?reason=failed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Momo PaymentCallback: {ex.Message}");
            return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=ServerError");
        }
    }

    private string HmacSha256(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hash).ToLower();
    }
}
