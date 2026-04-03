namespace GateWay.Models.Auth
{
    public class LoginRequest
    {
        // Dùng Identifier để có thể nhập Username hoặc Email đều được
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}