using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using BCrypt.Net;
using ORG.BasicInfo.API.Shared;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ORG.BasicInfo.Domain.UserAggregate;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.API.Features.Authentication.Response;
using Microsoft.Extensions.Options;
using ClassEncryptionLibrary;

namespace ORG.BasicInfo.API.Features.Authentication;

public class AuthenticationRequestHandler(
    FormsInfoDbContext dbContext,
    IValidator<AuthenticationRequest> validator,
    IRedisCacheService redisService,
    IOptionsMonitor<JwtOptions> optionsMonitor,
    IHttpContextAccessor httpContextAccessor,
    AesEncryption aesEncryptionSession
    ) : IRequestHandler<AuthenticationRequest, Result<ResponseAuth>>
{
    private readonly FormsInfoDbContext _dbContext = dbContext;
    private readonly IValidator<AuthenticationRequest> _validator = validator;
    private readonly IRedisCacheService _redisService= redisService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly JwtOptions _jwtOptions= optionsMonitor.Get("JwtAuth");
    private readonly AesEncryption _aesEncryptionSession = aesEncryptionSession;
    public async Task<Result<ResponseAuth>> Handle(AuthenticationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<ResponseAuth>.Invalid(validationResult.AsErrors());

        //Console.WriteLine(optionsMonitor.Get("JwtAuth"));
        var fundCode = request.FundCode;
        //var nationalCodeUser = request.NationalCodeUser;
        //var idCodeNationalFund = request.IdCodeNationalFund;

        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
             u => 
             //u.IdCodeNationalFund == idCodeNationalFund &&
             //u.NationalCodeUser ==Convert.ToUInt64(nationalCodeUser) &&
             u.FundCode == fundCode &&
             u.State == UserState.Active 
            , cancellationToken);
        var resultTmp=await user.GetEncryptDataWithThisKey("00000000-0000-0000-0000-000000000000_1");

        if (user == null)
            return Result<ResponseAuth>.NotFound("کاربر پیدا نشد و یا حساب کاربری غیر فعال گردیده");
        var mobile = user.Mobile;
        var verifyCodeHash = await _redisService.GetAsync<string>(mobile.ToString());
        if (verifyCodeHash == null)
            return Result<ResponseAuth>.NotFound("کد تایید یافت نشد یا منقضی شده است");
        string verifyCode = request.VerifyCode.ToString("D6");
        if (!BCrypt.Net.BCrypt.EnhancedVerify(verifyCode, verifyCodeHash, HashType.SHA512))
            return Result<ResponseAuth>.Error("کد تایید نادرست است.");
        string ipSystem=_httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (user.IpUser != "::"&& user.IpUser!= ipSystem)
        {
            return Result<ResponseAuth>.Error("ip نامعتبر" );
        }
        var claims = GenerateClaims(user);
        var refereshToken = CreateAccessToken(claims);

        // ایجاد و ذخیره session در Redis (همانطور که قبلاً داشتید)
        var nowUtc = DateTime.UtcNow;
        var sessionId = user.Id.ToString("N");
        var sessionTtlSeconds = _jwtOptions.ExpirationSeconds;
        //crsf token for each reqest
        var tknRole = await user.GetUserRole();
        var tknRoleStr = user.Id.ToString() + "_" + (tknRole).ToString();
        var session = new SessionEntry
        {
            //key crsf code
            SessionId = sessionId,
            fundCode=fundCode,
            UserId = user.Id.ToString(),
            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = nowUtc.AddSeconds(sessionTtlSeconds),
            Ip = ipSystem,
            UserAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
            UserRoleOrg =await _aesEncryptionSession.Encrypt(tknRoleStr)

        };
        var sessionSerialized = System.Text.Json.JsonSerializer.Serialize(session);
        await _redisService.SetAsync($"session_referesh:{sessionId}", sessionSerialized, TimeSpan.FromSeconds(sessionTtlSeconds));

        // ---------- ارسال کوکی ها با استفاده از _httpContextAccessor ----------
        var httpContext = _httpContextAccessor.HttpContext;
 

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTimeOffset.UtcNow.AddSeconds(sessionTtlSeconds),
            SameSite = SameSiteMode.None,
            Path = "/api/auth/refereshcode"
        };


        httpContext.Response.Cookies.Append("referesh_token", refereshToken, cookieOptions);
        httpContext.Response.Cookies.Append("session_referesh_id", sessionId, cookieOptions);

        return Result.Success(new ResponseAuth($"{user.Name} {user.LastName}-{user.FundName}",mobile.ToString().Substring(6,4)));
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

    private string CreateAccessToken(Claim[] claims)
    {
        var createdAt = DateTime.UtcNow;

        var expiresAt = createdAt.AddSeconds(_jwtOptions.ExpirationSeconds);

        var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.SigningKey);

        var symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
