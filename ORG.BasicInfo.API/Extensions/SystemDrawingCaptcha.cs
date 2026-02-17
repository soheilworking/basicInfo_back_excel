////using Microsoft.AspNetCore.Mvc;

////[ApiController]
////[Route("api/captcha")]
////public class CaptchaController : ControllerBase
////{
////    [HttpGet]
////    public IActionResult Get()
////    {
////        var code = CaptchaUtil.GenerateCode(6);
////        // ذخیره برای ولیدیشن; در این مثال داخل Session ذخیره می‌کنیم
////        HttpContext.Session.SetString("CaptchaCode", code);

////        var bytes = ImageSharpCaptcha.RenderPng(code);
////        return File(bytes, "image/png");
////    }

////    [HttpPost("verify")]
////    public IActionResult Verify([FromForm] string userInput)
////    {
////        var expected = HttpContext.Session.GetString("CaptchaCode");
////        HttpContext.Session.Remove("CaptchaCode");
////        if (string.IsNullOrEmpty(expected)) return BadRequest("Captcha expired");
////        return Ok(new { success = string.Equals(expected, userInput, StringComparison.OrdinalIgnoreCase) });
////    }
////}
//using System;
//using System.Text.Json;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;

//[ApiController]
//[Route("api/captcha")]
//

//public class CaptchaController : ControllerBase
//{
//    private const string SessionKey = "CaptchaPayload";
//    private const int ExpireMinutes = 2;

//    [HttpGet]
//    public IActionResult Get()
//    {
//        var code = CaptchaUtil.GenerateCode(6); // متد تولید کد 6 کاراکتری
//        var ip = GetRequestIp(HttpContext);
//        var payload = new CaptchaPayload(code, ip, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

//        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(payload));

//        var bytes = ImageSharpCaptcha.RenderPng(code); // متد تولید تصویر از کد
//        return File(bytes, "image/png");
//    }

//    [HttpPost("verify")]
//    public IActionResult Verify([FromForm] string userInput)
//    {
//        if (string.IsNullOrWhiteSpace(userInput))
//            return BadRequest(new { success = false, reason = "invalid_input" });

//        var json = HttpContext.Session.GetString(SessionKey);
//        HttpContext.Session.Remove(SessionKey); // one-time use

//        if (string.IsNullOrEmpty(json))
//            return BadRequest(new { success = false, reason = "missing_or_expired" });

//        CaptchaPayload? payload;
//        try
//        {
//            payload = JsonSerializer.Deserialize<CaptchaPayload>(json);
//        }
//        catch
//        {
//            return BadRequest(new { success = false, reason = "invalid_payload" });
//        }

//        if (payload == null)
//            return BadRequest(new { success = false, reason = "invalid_payload" });

//        // بررسی زمان انقضا
//        var created = DateTimeOffset.FromUnixTimeSeconds(payload.CreatedAtUnix);
//        if (DateTimeOffset.UtcNow - created > TimeSpan.FromMinutes(ExpireMinutes))
//            return BadRequest(new { success = false, reason = "expired" });

//        // بررسی IP
//        var requestIp = GetRequestIp(HttpContext);
//        if (!string.Equals(payload.Ip, requestIp, StringComparison.OrdinalIgnoreCase))
//            return BadRequest(new { success = false, reason = "ip_mismatch" });

//        // مقایسه کد (Case-insensitive)
//        bool ok = string.Equals(payload.Code, userInput.Trim(), StringComparison.OrdinalIgnoreCase);
//        return Ok(new { success = ok });
//    }

//    private static string GetRequestIp(HttpContext ctx)
//    {
//        if (ctx.Request.Headers.TryGetValue("X-Forwarded-For", out var header) && !string.IsNullOrWhiteSpace(header))
//        {
//            var first = header.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
//            if (!string.IsNullOrEmpty(first)) return first;
//        }

//        var ip = ctx.Connection.RemoteIpAddress;
//        if (ip == null) return "unknown";
//        if (ip.IsIPv4MappedToIPv6) ip = ip.MapToIPv4();
//        return ip.ToString();
//    }
//}

////var builder = WebApplication.CreateBuilder(args);

////builder.Services.AddDistributedMemoryCache(); // برای Session backend
////builder.Services.AddSession(options =>
////{
////    options.IdleTimeout = TimeSpan.FromMinutes(20); // تنظیم کلی Session
////    options.Cookie.HttpOnly = true;
////    options.Cookie.IsEssential = true;
////});

////builder.Services.AddControllers();
////var app = builder.Build();

////app.UseSession();
////app.MapControllers();
////app.Run();
