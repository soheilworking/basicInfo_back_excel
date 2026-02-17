using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using ORG.BasicInfo.API.Shared;
using ORG.BasicInfo.Data;
namespace ORG.BasicInfo.API.Features.Authentication;
public class LogoutRequestHandler(
    FormsInfoDbContext dbContext,
    IRedisCacheService redisService,
    IOptionsMonitor<JwtOptions> optionsMonitor,
    IHttpContextAccessor httpContextAccessor
    ) : IRequestHandler<LogoutRequest, Result<bool>>
{

    private readonly IRedisCacheService _redisService = redisService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    string _sessionRefereshCookieName = "session_referesh_id";
    string _sessionCookieName = "sessionId";


    public async Task<Result<bool>> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;


        if (!httpContext.Request.Cookies.TryGetValue(_sessionRefereshCookieName, out var sessionRefereshId) || string.IsNullOrEmpty(sessionRefereshId))
        {

            return Result<bool>.Unauthorized("توکن نامعتبر است.");

        }
        if (!httpContext.Request.Cookies.TryGetValue(_sessionCookieName, out var sessionId) || string.IsNullOrEmpty(sessionId))
        {

            return Result<bool>.Unauthorized("توکن نامعتبر است.");

        }

        await _redisService.RemoveAsync($"session_referesh:{sessionRefereshId}");
        await _redisService.RemoveAsync($"session:{sessionId}");
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/api/auth/refereshcode"
        };
        var cookieOptions_access = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };
        httpContext.Response.Cookies.Delete("sessionId", cookieOptions_access);
        httpContext.Response.Cookies.Delete("session_referesh_id", cookieOptions);
        httpContext.Response.Cookies.Delete("referesh_token", cookieOptions);
        httpContext.Response.Cookies.Delete("access_token", cookieOptions_access);

        return Result<bool>.Success(true);
    }
}
