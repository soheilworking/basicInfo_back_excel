using ORG.BasicInfo.API.Features.Abstractions;
using System.Text.Json;

namespace ORG.BasicInfo.API.Features.Authentication
{
    public class CheckCaptcha
    {
        public static string CheckCapchaCode(string CpachaCode, HttpContext ctx)
        {
            if (string.IsNullOrWhiteSpace(CpachaCode))
                return "ورودی نامعتبر کد امنیتی";
            var requestIpKey = GetRequestIp(ctx);
            var json = ctx.Session.GetString(requestIpKey);
            ctx.Session.Remove(requestIpKey); // one-time use

            if (string.IsNullOrEmpty(json))
                return "کد امنیتی نامعتبر است";

            CaptchaPayload? payload;
            try
            {
                payload = JsonSerializer.Deserialize<CaptchaPayload>(json);
            }
            catch
            {
                return "کد امنیتی نامعتبر است.";
            }

            if (payload == null)
                return "کد امنیتی نامعتبر است.";

            // بررسی زمان انقضا
            var created = DateTimeOffset.FromUnixTimeSeconds(payload.CreatedAtUnix);
            if (DateTimeOffset.UtcNow - created > TimeSpan.FromMinutes(2))
                return "کد امنیتی نامعتبر است";

            // بررسی IP
            var requestIp = GetRequestIp(ctx);
            if (!string.Equals(payload.Ip, requestIp, StringComparison.OrdinalIgnoreCase))
                return "کد امنیتی نامعتبر است";

            // مقایسه کد (Case-insensitive)
            bool ok = string.Equals(payload.Code, CpachaCode.Trim(), StringComparison.OrdinalIgnoreCase);
            if(ok==false)
                return "کد امنیتی نامعتبر است";

            return "";
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
}
