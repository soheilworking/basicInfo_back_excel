using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ORG.BasicInfo.API.Features.Abstractions;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Authentication.Response;
namespace ORG.BasicInfo.API.Features.Authentication;

public class GetCpachaCodeRequestHandler(

    IHttpContextAccessor httpContextAccessor
    ) : IRequestHandler<GetCpachaCodeRequest, Result<ResponseFile>>
{
    private readonly IHttpContextAccessor _httpContextAccessor= httpContextAccessor;
    private const string SessionKey = "CaptchaPayload";
    public async Task<Result<ResponseFile>> Handle(GetCpachaCodeRequest request, CancellationToken cancellationToken)
    {
        var code = CaptchaUtil.GenerateCode(6); // متد تولید کد 6 کاراکتری
        var ip = GetRequestIp(_httpContextAccessor.HttpContext);
        var payload = new CaptchaPayload(code, ip, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        _httpContextAccessor.HttpContext.Session.SetString(ip, JsonSerializer.Serialize(payload));

        var bytes = ImageSharpCaptcha.RenderPng(code); // متد تولید تصویر از کد
        return Result.Success(new ResponseFile(bytes, "image/png"));
    }

    private static string GetRequestIp(HttpContext ctx)
    {
        if (ctx.Request.Headers.TryGetValue("X-Forwarded-For", out var header) && !string.IsNullOrWhiteSpace(header))
        {
            var first = header.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            if (!string.IsNullOrEmpty(first)) return first;
        }

        var ip = ctx.Connection.RemoteIpAddress;
        if (ip == null) return "unknown";
        if (ip.IsIPv4MappedToIPv6) ip = ip.MapToIPv4();
        return ip.ToString();
    }
}

    // var bytes = System.Text.Encoding.UTF8.GetBytes(value);
    //await _distributedCache.SetAsync(key, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });


   //[HttpPost("set/{key}")]
   // public async Task<IActionResult> Set(string key, [FromBody] object value)
   // {
   //     await _redisService.SetAsync(key, value, TimeSpan.FromMinutes(10));
   //     return NoContent();
   // }

   // [HttpGet("get/{key}")]
   // public async Task<IActionResult> Get(string key)
   // {
   //     var obj = await _redisService.GetAsync<object>(key);
   //     if (obj == null) return NotFound();
   //     return Ok(obj);
   // }