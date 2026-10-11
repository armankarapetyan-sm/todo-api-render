namespace TodoApi.Models;

public class TokenResponse
{
    public string AccessToken { get; set; } = "";
    public string TokenType { get; set; } = "Bearer";
    public DateTimeOffset ExpiresUtc { get; set; }
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
}
