using System.Net;
using System.Security.Cryptography;
using System.Text;
using API_WebH3.DTOs.Order;
using API_WebH3.Repositories;

namespace API_WebH3.Services;

public class VnpayService
{
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orderRepository;
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());
    
    
    public VnpayService(IConfiguration configuration, IOrderRepository orderRepository)
    {
        _configuration = configuration;
        _orderRepository = orderRepository;
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
        _requestData.Add("vnp_TxnRef", orderDto.Id.ToString()); // Dùng Id từ OrderDto

        var paymentUrl = CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
        return paymentUrl;
    }

    public async Task<OrderDto> PaymentExecuteAsync(IQueryCollection collections)
    {
        foreach (var (key, value) in collections)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                _responseData.Add(key, value);
            }
        }

        var orderId = Guid.Parse(GetResponseData("vnp_TxnRef")); // Chuyển về Guid từ vnp_TxnRef
        var vnpResponseCode = GetResponseData("vnp_ResponseCode");
        var vnpSecureHash = collections["vnp_SecureHash"];
        var orderInfo = GetResponseData("vnp_OrderInfo");
        var checkSignature = ValidateSignature(vnpSecureHash, _configuration["Vnpay:HashSecret"]);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || !checkSignature)
        {
            return new OrderDto { Status = "Failed" };
        }

        // Chuyển đổi Order sang OrderDto
        var orderDto = new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };

        // Cập nhật trạng thái đơn hàng nếu thanh toán thành công
        if (vnpResponseCode == "00")
        {
            order.Status = "Paid";
            await _orderRepository.UpdateAsync(order);
            orderDto.Status = "Paid";
        }
        else
        {
            orderDto.Status = "Failed";
        }
        
        if (vnpResponseCode == "00")
        {
            order.Status = "Paid";
            await _orderRepository.UpdateAsync(order);
            orderDto.Status = "Paid";
        }
        else if (vnpResponseCode == "24")
        {
            order.Status = "Cancelled";
            await _orderRepository.UpdateAsync(order);
            orderDto.Status = "Cancelled";
        }
        else
        {
            order.Status = "Failed";
            await _orderRepository.UpdateAsync(order);
            orderDto.Status = "Failed";
        }

        return orderDto;
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

            if (data.Length > 0) data.Length -= 1; // Xóa "&" cuối cùng
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