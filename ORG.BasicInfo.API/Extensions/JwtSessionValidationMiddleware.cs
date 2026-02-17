using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClassEncryptionLibrary;
using Mapster;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Authentication.Response;

public class JwtSessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRedisCacheService _redisService;
    private readonly string _sessionCookieName;

    private readonly AesEncryption _aesEncryptionSession;
    public JwtSessionValidationMiddleware(RequestDelegate next, IRedisCacheService redisService,
        AesEncryption aesEncryptionSession,
        string sessionCookieName = "session_id")
    {
        _next = next;
        _redisService = redisService;
        _sessionCookieName = sessionCookieName;
        _aesEncryptionSession = aesEncryptionSession;


    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        // بررسی وجود صفت مارکر
        var hasMarker = endpoint?.Metadata?.GetMetadata<RequireSessionValidationAttribute>() != null;



        // اگر نه مارکر و نه scheme هدف وجود دارد، عبور سریع
        if (!hasMarker)
        {
            try
            {
                // لاگ قبل از ادامه (اختیاری)
                // logger?.LogDebug("JwtSessionValidationMiddleware: skipping validation for {Path}", context.Request.Path);
                //if(_next(context).IsCompletedSuccessfully==true)
                await _next(context).ConfigureAwait(false);

                // هیچ تغییری در response پس از برگشت انجام نده
                return;
            }
            catch (Exception ex)
            {
                // فقط لاگ کن و دوباره پرتاب کن؛ هرگز تلاش نکن پاسخ را تغییر دهی اگر قبلاً شروع شده
                // logger?.LogError(ex, "Downstream exception when middleware skipped for {Path}", context.Request.Path);
                throw;
            }
        }

        // از اینجا منطق session-check اجرا می‌شود
        if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
        //var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        //var userId = Guid.Parse(claim.Value);

        var userIdSession = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? context.User.FindFirst("user_id")?.Value;
        if (string.IsNullOrEmpty(userIdSession))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid token claims");
            return;
        }

        if (!context.Request.Cookies.TryGetValue(_sessionCookieName, out var sessionId) || string.IsNullOrEmpty(sessionId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Session not found");
            return;
        }

        // واکشی از Redis بر اساس sessionId (تو از این شیوه استفاده کردی)
        var storedValueString = await _redisService.GetAsync<string>($"session:{sessionId}");
        var storedValueObj = FromJson(storedValueString);
        //var ipSystem = context.Connection?.RemoteIpAddress?.ToString();
        //if (storedValueObj.Ip!="::"&& storedValueObj.Ip != ipSystem)
        //{
        //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //    await context.Response.WriteAsync("ip not valid");
        //    return;
        //}
       
            if (storedValueObj == null ||
            string.IsNullOrEmpty(storedValueString) ||
            storedValueObj.SessionId != sessionId ||
            storedValueObj.UserId != userIdSession ||
             storedValueObj.Ip != context.Connection?.RemoteIpAddress?.ToString() ||
            storedValueObj.UserAgent != context.Request?.Headers["User-Agent"].ToString())
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid session");
            return;
        }
        //if (storedValueObj.fundCode.ToString()!=context.Items["fundCode"].ToString())
        //{
        //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //    await context.Response.WriteAsync("گواهینامه غیر مجاز");
        //    return;
        //}
        string[] roleTmp;
        var tmp23=await _aesEncryptionSession.Decrypt(storedValueObj.UserRoleOrg);
        ushort roleNum = 0;
        try
        {
            roleTmp =(await _aesEncryptionSession.Decrypt(storedValueObj.UserRoleOrg)).Split("_");
            if (roleTmp.Length > 1 && roleTmp[0]== userIdSession)
            {
                roleNum = Convert.ToUInt16(roleTmp[1]);
            }
            else
            {
                await context.Response.WriteAsync("Invalid session");
                return;
            }
        }
        catch (Exception)
        {
            await context.Response.WriteAsync("Invalid session");
            return;
        }

      

        // قرار دادن اطلاعات برای استفاده در کنترلرها
        context.Items["UserId"] = userIdSession;
        context.Items["SessionId"] = sessionId;
        context.Items["IsRoleOrg"] = roleNum;
        
        context.Items["SessionEntry"] = storedValueObj;
        context.Items["IpUser"] = storedValueObj.Ip;
        if(roleNum==2)
            context.Items["PermissionFunds"] = storedValueObj.PerFunds;
        else if(roleNum == 3)
            context.Items["PermissionFunds"] = userIdSession == null ? (IEnumerable< Guid>) [Guid.Empty] : (IEnumerable<Guid>)[Guid.Parse(userIdSession)];

        await _next(context);
    }

    public static SessionEntry? FromJson(string? json)
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
}
