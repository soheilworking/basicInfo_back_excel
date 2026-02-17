using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ardalis.Result;
using ORG.BasicInfo.API.Shared;
using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ORG.BasicInfo.Domain.UserAggregate;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.API.Features.Authentication.Response;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ORG.BasicInfo.API.Features.Abstractions;
using ClassEncryptionLibrary;
namespace ORG.BasicInfo.API.Features.Authentication;

public class GetAccessCodesRequestHandler(
    FormsInfoDbContext dbContext,
    IValidator<GetAccessCodesRequest> validator,
    IRedisCacheService redisService,
    IOptionsMonitor<JwtOptions> optionsMonitor,
    IHttpContextAccessor httpContextAccessor,
    AesEncryption aesEncryption
    ) : IRequestHandler<GetAccessCodesRequest, Result<ushort>>
{
    private readonly FormsInfoDbContext _dbContext = dbContext;
    private readonly JwtOptions _jwtOptions = optionsMonitor.Get("JwtAuth");
    private readonly JwtOptions _jwtOptionsAceess = optionsMonitor.Get("JwtAccessScheme");
    private readonly IValidator<GetAccessCodesRequest> _validator = validator;
    private readonly IRedisCacheService _redisService= redisService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly AesEncryption _aesEncryption = aesEncryption;
    string _sessionCookieName = "session_referesh_id";
    string _tokenCookieName = "referesh_token";
    
    public async Task<Result<ushort>> Handle(GetAccessCodesRequest request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        // خواندن توکن و sessionId از کوکی‌ها
        if (!httpContext.Request.Cookies.TryGetValue(_tokenCookieName, out var sessionToken) || string.IsNullOrEmpty(sessionToken))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Session not found");
            return Result<ushort>.Unauthorized("توکن نامعتبر است.");
        }

        if (!httpContext.Request.Cookies.TryGetValue(_sessionCookieName, out var sessionId) || string.IsNullOrEmpty(sessionId))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Session not found");
            return Result<ushort>.Unauthorized("توکن نامعتبر است.");
        }

        // اعتبارسنجی JWT و استخراج claim مربوط به شناسه کاربر
        ClaimsPrincipal principal;
        try
        {
            principal = ValidateJwtToken(sessionToken);
        }
        catch (SecurityTokenException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Invalid token");
            return Result<ushort>.Unauthorized("توکن نامعتبر است.");
        }
        catch (Exception)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Invalid token");
            return Result<ushort>.Unauthorized("توکن نامعتبر است.");
        }

        var nameIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst("sub") ?? principal.FindFirst("nameid");
        if (nameIdClaim == null || string.IsNullOrEmpty(nameIdClaim.Value))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Invalid token claims");
            return Result<ushort>.Unauthorized("توکن نامعتبر است.");
        }

        if (!Guid.TryParse(nameIdClaim.Value, out var userIdJwt))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Invalid user id in token");
            return Result<ushort>.Unauthorized("توکن نامعتبر است.");
        }        //session_referesh
        var storedValueString = await _redisService.GetAsync<string>($"session_referesh:{sessionId}");
        var storedValueObj = FromJson(storedValueString);
        if (storedValueObj == null ||
        string.IsNullOrEmpty(storedValueString) ||
        storedValueObj.SessionId != sessionId ||
        Guid.Parse(storedValueObj.UserId) != userIdJwt ||
        storedValueObj.Ip != httpContext.Connection?.RemoteIpAddress?.ToString() ||
        storedValueObj.UserAgent != httpContext.Request?.Headers["User-Agent"].ToString())
        {
            return Result<ushort>.Unauthorized("توکن نامعتبر است.");
        }




        var nowUtc = DateTime.UtcNow;
        //Guid g2 = GuidTransformer.TransformGuid(g);
        var sessionIdNew = GuidTransformer.TransformGuid(userIdJwt).ToString("N");
        //var sessionIdNew = Guid.NewGuid().ToString("N");
        var sessionTtlSeconds = _jwtOptions.ExpirationSeconds;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTimeOffset.UtcNow.AddSeconds(sessionTtlSeconds),
            SameSite = SameSiteMode.None,
            Path = "/"
        };
        var user = await _dbContext.Users.FirstOrDefaultAsync(item => item.Id == userIdJwt);
        if (user == null)
            return Result<ushort>.Unauthorized("توکن نامعتبر است.");
        var pidUserDec=await PermissionFund.GetEncryptDataWithThisKey(user.Id.ToString());

        if(pidUserDec==null)
            return Result<ushort>.Unauthorized("توکن نامعتبر است.");

        var permissionFunds = await _dbContext.PermissionFunds.Where(item => item.IdUser == pidUserDec).ToListAsync(); 
        var fundIds = await Task.WhenAll(permissionFunds.Select(item => item.GetIdFund()));

        var claims = GenerateClaims(user);
        var accessToken = CreateAccessToken(claims);
        var session = new SessionEntry
        {
            //key crsf code
            SessionId = sessionIdNew,
            UserId = user.Id.ToString(),
            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = nowUtc.AddSeconds(sessionTtlSeconds),
            Ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
            UserAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
            UserRoleOrg =await _aesEncryption.Encrypt(user.Id.ToString() + "_" + (await user.GetUserRole()).ToString()),
            PerFunds= fundIds
        };
        var sessionSerialized = System.Text.Json.JsonSerializer.Serialize(session);
        await _redisService.SetAsync($"session:{sessionIdNew}", sessionSerialized, TimeSpan.FromSeconds(sessionTtlSeconds));

        httpContext.Response.Cookies.Append("sessionId", sessionIdNew, cookieOptions);
        // واکشی از Redis بر اساس sessionId (تو از این شیوه استفاده کردی)

        httpContext.Response.Cookies.Append("access_token", accessToken, cookieOptions);
        // قرار دادن اطلاعات برای استفاده در کنترلرها
     

        return Result<ushort>.Success((await user.GetUserRole()));
    }
    private static Claim[] GenerateClaims(User user)
    {
        var identifier = user.Id.ToString();

        return
        [
            new Claim(ClaimTypes.NameIdentifier, identifier),
            new Claim(JwtRegisteredClaimNames.UniqueName, identifier),
            new Claim(JwtRegisteredClaimNames.Sub, user.Name, ClaimValueTypes.String),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
    }
    private ClaimsPrincipal ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (string.IsNullOrEmpty(_jwtOptions.SigningKey))
            throw new InvalidOperationException("JWT secret is not configured.");

        var key = Encoding.UTF8.GetBytes(_jwtOptions.SigningKey);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = !string.IsNullOrEmpty(_jwtOptions.Issuer),
            ValidIssuer = _jwtOptions.Issuer,
            ValidateAudience = !string.IsNullOrEmpty(_jwtOptions.Audience),
            ValidAudience = _jwtOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        SecurityToken validatedToken;
        var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        return principal;
    }
    private static SessionEntry? FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            return JsonSerializer.Deserialize<SessionEntry>(json, options);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private string CreateAccessToken(Claim[] claims)
    {
        var createdAt = DateTime.UtcNow;

        var expiresAt = createdAt.AddSeconds(_jwtOptionsAceess.ExpirationSeconds);

        var keyBytes = Encoding.UTF8.GetBytes(_jwtOptionsAceess.SigningKey);

        var symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtOptionsAceess.Issuer,
            audience: _jwtOptionsAceess.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
