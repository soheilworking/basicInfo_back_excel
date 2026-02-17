namespace ORG.BasicInfo.API.Shared;

public class JwtOptions
{
    public string SigningKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationSeconds { get; set; }
}