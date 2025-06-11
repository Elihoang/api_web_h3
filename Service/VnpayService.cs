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
using Microsoft.Extensions.Configuration;
using System.IO;

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
    private readonly ICouponRepository _couponRepository;

    public VnpayService(
        IConfiguration configuration,
        OrderService orderService,
        EnrollmentService enrollementService,
        EmailPaymentService emailPaymentService,
        ICouponRepository couponRepository,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _orderService = orderService;
        _enrollementService = enrollementService;
        _emailPaymentService = emailPaymentService;
        _userRepository = userRepository;
        _couponRepository = couponRepository;
        ValidateConfiguration();
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_configuration["Vnpay:TmnCode"])) throw new ArgumentException("Vnpay:TmnCode is not configured.");
        if (string.IsNullOrEmpty(_configuration["Vnpay:HashSecret"])) throw new ArgumentException("Vnpay:HashSecret is not configured.");
        if (string.IsNullOrEmpty(_configuration["Vnpay:BaseUrl"])) throw new ArgumentException("Vnpay:BaseUrl is not configured.");
        if (string.IsNullOrEmpty(_configuration["Vnpay:PaymentBackReturnUrl"])) throw new ArgumentException("Vnpay:PaymentBackReturnUrl is not configured.");
        if (string.IsNullOrEmpty(_configuration["Frontend:BaseUrl"])) throw new ArgumentException("Frontend:BaseUrl is not configured.");
        if (string.IsNullOrEmpty(_configuration["EmailTemplate:Path"])) throw new ArgumentException("EmailTemplate:Path is not configured.");
        Console.WriteLine("VnPay, Frontend, and Email configuration validated successfully.");
    }

    public string CreatePaymentUrl(OrderDto order, HttpContext context)
    {
        try
        {
            if (order.Amount < 0 || string.IsNullOrEmpty(order.Id) || order.UserId == Guid.Empty)
            {
                AppLogger.LogError($"Invalid order data: Amount={order.Amount}, Id={order.Id}, UserId={order.UserId}");
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
            
            string dataRequest = "";
            foreach (var (key, value) in _requestData)
            {
                dataRequest += $"{key}: {value}\n";
            }
            AppLogger.LogInfo($"Request data for payment URL: {dataRequest}");
            return CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Error in CreatePaymentUrl: {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<IActionResult> PaymentExecuteAsync(IQueryCollection collections)
    {
        try
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
                AppLogger.LogError("Missing vnp_TxnRef in response data.");
                return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=MissingOrderId");
            }

            var txnRefRaw = GetResponseData("vnp_TxnRef");
            if (string.IsNullOrWhiteSpace(txnRefRaw))
            {
                AppLogger.LogError($"Invalid vnp_TxnRef format: {GetResponseData("vnp_TxnRef")}");
                return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=InvalidOrderId");
            }
            var orderId = txnRefRaw;

            var vnpResponseCode = GetResponseData("vnp_ResponseCode");
            var vnpSecureHash = collections["vnp_SecureHash"];
            var orderInfo = GetResponseData("vnp_OrderInfo");

            if (string.IsNullOrEmpty(vnpSecureHash))
            {
                AppLogger.LogError("Missing vnp_SecureHash in response data.");
                return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=MissingSignature");
            }

            var checkSignature = ValidateSignature(vnpSecureHash, _configuration["Vnpay:HashSecret"]);
            AppLogger.LogInfo($"Signature validation: {checkSignature}");

            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
            {
                AppLogger.LogError($"Order not found: Id={orderId}");
                return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=OrderNotFound");
            }

            if (!checkSignature)
            {
                AppLogger.LogError($"Invalid signature for order: Id={orderId}");
                return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=InvalidSignature");
            }

            string redirectUrl;
            switch (vnpResponseCode)
            {
                case "00":
                    AppLogger.LogSuccess($"Payment successful for order: Id={orderId}");
                    await _orderService.UpdateOrderStatus(orderId, "Paid");

                    foreach (var detail in order.OrderDetails)
                    {
                        var existingEnrollment = await _enrollementService.GetByUserAndCourseAsync(order.UserId, detail.CourseId);
                        if (existingEnrollment == null)
                        {
                            AppLogger.LogInfo($"Creating enrollment for UserId={order.UserId}, CourseId={detail.CourseId}");
                            await _enrollementService.CreateAsync(new CreateEnrollmentDto
                            {
                                UserId = order.UserId,
                                CourseId = detail.CourseId,
                                Status = "Enrolled"
                            });
                        }
                    }

                    var user = await _userRepository.GetByIdAsync(order.UserId);
                    if (user == null)
                    {
                        AppLogger.LogError($"User not found: UserId={order.UserId}");
                        redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-success/{orderId}" +
                                      $"?amount={GetResponseData("vnp_Amount")}" +
                                      $"&orderInfo={WebUtility.UrlEncode(orderInfo)}" +
                                      $"&responseCode={vnpResponseCode}";
                        return new RedirectResult(redirectUrl);
                    }

                    if (string.IsNullOrEmpty(user.Email))
                    {
                        AppLogger.LogError($"User email is empty: UserId={order.UserId}");
                        redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-success/{orderId}" +
                                     $"?vnp_Amount={GetResponseData("vnp_Amount")}" +
                                     $"&vnp_OrderInfo={WebUtility.UrlEncode(orderInfo)}" +
                                     $"&vnp_ResponseCode={vnpResponseCode}";
                        return new RedirectResult(redirectUrl);
                    }

                    AppLogger.LogInfo($"Found user: Id={user.Id}, Email={user.Email}");


                    // Send confirmation email
                    var orderDetail = await _orderService.GetOrderDetailsByOrderIdAsync(order.Id);
                    var instructor = await _userRepository.GetByIdAsync(orderDetail.FirstOrDefault().Course.InstructorId);
                    try
                    {
                        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), _configuration["EmailTemplate:Path"]);
                        AppLogger.LogError($"Attempting to read email template from: {templatePath}");
                        if (!File.Exists(templatePath))
                        {
                            AppLogger.LogError($"Email template not found at: {templatePath}");
                            throw new FileNotFoundException($"Email template file not found at {templatePath}");
                        }
                        var subject = $"Thanh toán thành công - Đơn hàng #{order.Id}";
                        var templateContent = await File.ReadAllTextAsync(templatePath);
                        var paymentDate = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");
                        var courseAccessUrl =
                            $"{_configuration["Frontend:BaseUrl"]}/details/{orderDetail.FirstOrDefault().CourseId}";
                        var body = templateContent
                            .Replace("{invoiceId}", order.Id)
                            .Replace("{courseName}", orderDetail.FirstOrDefault()?.Course?.Title?? "Khóa học không xác định")
                            .Replace("{issueDate}", order.CreatedAt)
                            .Replace("{studentName}", user.FullName)
                            .Replace("{studentEmail}", user.Email)
                            .Replace("{receiverEmail}", user.Email)
                            .Replace("{transactionId}", order.Id)
                            .Replace("{amount}", $"{order.Amount:N0} VND")
                            .Replace("{paymentDate}", paymentDate)
                            .Replace("{paymentMethod}", "VNPay")
                            .Replace("{instructorName}", instructor.FullName)
                            .Replace("{startDate}", paymentDate)
                            .Replace("{courseAccessUrl}", courseAccessUrl);

                        AppLogger.LogInfo($"Email body prepared: {body}");
                        await _emailPaymentService.SendEmailAsync(user.Email, subject, body);
                        AppLogger.LogSuccess($"Email sent successfully to {user.Email}");
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogError($"Failed to send email to {user.Email}: {ex.Message}\nStackTrace: {ex.StackTrace}");
                        // Continue with success redirect even if email fails
                    }

                    redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-success/{orderId}" +
                                 $"?vnp_Amount={GetResponseData("vnp_Amount")}" +
                                 $"&vnp_OrderInfo={WebUtility.UrlEncode(orderInfo)}" +
                                 $"&vnp_ResponseCode={vnpResponseCode}";
                    break;
                case "24":
                    AppLogger.LogInfo($"Payment cancelled for order: Id={orderId}");
                    await _orderService.UpdateOrderStatus(orderId, "Cancelled");
                    redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-failure?reason=cancelled";
                    break;
                default:
                    AppLogger.LogError($"Payment failed for order: Id={orderId}, ResponseCode={vnpResponseCode}");
                    await _orderService.UpdateOrderStatus(orderId, "Failed");
                    redirectUrl = $"{_configuration["Frontend:BaseUrl"]}/payment-failure?reason=failed&vnp_ResponseCode={vnpResponseCode}";
                    break;
            }

            AppLogger.LogSuccess($"Redirect URL: {redirectUrl}");
            return new RedirectResult(redirectUrl);
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Error in PaymentExecuteAsync: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=ServerError&message={WebUtility.UrlEncode(ex.Message)}");
        }
    }

    private string GetIpAddress(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ip) || ip == "::1")
        {
            ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "127.0.0.1";
        }
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