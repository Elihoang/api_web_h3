namespace API_WebH3.MomoPayment;

public class MomoResponse
{
    public int resultCode { get; set; }
    public string message { get; set; }
    public string payUrl { get; set; }
    public string deeplink { get; set; }
    public string qrCodeUrl { get; set; }
}
