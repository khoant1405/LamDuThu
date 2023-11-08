namespace JSN.Shared.Config;

public class JwtConfig
{
    public string? ValidAudience { get; set; }
    public string? ValidIssuer { get; set; }
    public string? Token { get; set; }
    public int TokenValidityInMinutes { get; set; }
    public int RefreshTokenValidityInDays { get; set; }
}