using System.Net;
using System.Security.Cryptography;
using System.Text;
using API_WebH3.DTOs.Enrollment;
using API_WebH3.DTOs.Order;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Services;

public class VnpayService
{
    private readonly IConfiguration _configuration;
    private readonly OrderService _orderService;
    private readonly EnrollementService _enrollementService;
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

    public VnpayService(
        IConfiguration configuration,
        OrderService orderService,
        EnrollementService enrollementService)
    {
        _configuration = configuration;
        _orderService = orderService;
        _enrollementService = enrollementService;
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

    public string CreatePaymentUrl(OrderDto orderDto, HttpContext context)
    {
        if (orderDto.Amount < 0 || orderDto.Id == Guid.Empty || orderDto.UserId == Guid.Empty || orderDto.CourseId == Guid.Empty)
        {
            Console.WriteLine($"Invalid order data: Amount={orderDto.Amount}, Id={orderDto.Id}, UserId={orderDto.UserId}, CourseId={orderDto.CourseId}");
            throw new ArgumentException("Dữ liệu đơn hàng không hợp lệ: Amount, Id, UserId hoặc CourseId không đúng.");
        }

        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"] ?? "SE Asia Standard Time");
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

        _requestData.Clear();
        _requestData.Add("vnp_Version", _configuration["Vnpay:Version"] ?? "2.1.0");
        _requestData.Add("vnp_Command", _configuration["Vnpay:Command"] ?? "pay");
        _requestData.Add("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
        _requestData.Add("vnp_Amount", ((int)(orderDto.Amount * 100)).ToString());
        _requestData.Add("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        _requestData.Add("vnp_CurrCode", _configuration["Vnpay:CurrCode"] ?? "VND");
        _requestData.Add("vnp_IpAddr", GetIpAddress(context));
        _requestData.Add("vnp_Locale", _configuration["Vnpay:Locale"] ?? "vn");
        _requestData.Add("vnp_OrderInfo", $"Thanh toán đơn hàng #{orderDto.Id}");
        _requestData.Add("vnp_OrderType", "billpayment");
        _requestData.Add("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
        _requestData.Add("vnp_TxnRef", orderDto.Id.ToString());

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
        if (!Guid.TryParse(GetResponseData("vnp_TxnRef"), out var orderId))
        {
            Console.WriteLine($"Invalid vnp_TxnRef format: {GetResponseData("vnp_TxnRef")}");
            return new RedirectResult($"{_configuration["Frontend:BaseUrl"]}/payment-failure?error=InvalidOrderId");
        }

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

                var existingEnrollment = await _enrollementService.GetByUserAndCourseAsync(order.UserId, order.CourseId);
                if (existingEnrollment == null)
                {
                    Console.WriteLine($"Creating enrollment for UserId={order.UserId}, CourseId={order.CourseId}");
                    await _enrollementService.CreateAsync(new CreateEnrollmentDto
                    {
                        UserId = order.UserId,
                        CourseId = order.CourseId,
                        Status = "Active"
                    });
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