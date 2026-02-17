namespace ORG.BasicInfo.API.Features.Authentication.Response;

public record TokenResponse(string AccessToken, int ExpiresIn,  string SessionId );
public class SessionEntry
{
    public string SessionId { get; set; }
    public ulong fundCode { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public string Ip { get; set; }
    public string UserAgent { get; set; }
    public string UserRoleOrg { get; set; }
    
    public IEnumerable<Guid> PerFunds { get; set; }
}
