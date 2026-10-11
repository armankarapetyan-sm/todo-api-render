namespace TodoApi.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "TodoApi";
    public string Audience { get; set; } = "TodoApi";
    public string Key { get; set; } = "";
    public int LifetimeMinutes { get; set; } = 60;
}
