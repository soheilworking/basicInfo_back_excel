using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ORG.BasicInfo.API.Extensions
{
    public class CertificateFundCodeMiddleware
    {
        private readonly RequestDelegate _next;

        public CertificateFundCodeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var cert = context.Connection.ClientCertificate;

            if (cert != null)
            {
                string? upn = null;

                // 1) تلاش اول: گرفتن UPN مستقیم
                try
                {
                    upn = cert.GetNameInfo(X509NameType.UpnName, false);
                }
                catch
                {
                    // اگر SAN ساختار خاصی داشته باشد، این متد ممکن است خطا بدهد
                }

                // 2) اگر UPN مستقیم نبود، از SAN بخوان
                if (string.IsNullOrWhiteSpace(upn))
                {
                    var sanExt = cert.Extensions["2.5.29.17"];
                    if (sanExt != null)
                    {
                        try
                        {
                            var asn = new AsnEncodedData(sanExt.Oid, sanExt.RawData);
                            string sanText = asn.Format(true);

                            // مثال خروجی:
                            // DNS Name=localhost
                            // Other Name: Principal Name=soheil@domain.com
                            foreach (var line in sanText.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (line.Contains("Principal Name=", StringComparison.OrdinalIgnoreCase))
                                {
                                    upn = line.Split('=')[1].Trim();
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            // اگر SAN خراب باشد، اینجا خطا نمی‌دهیم
                        }
                    }
                }

                // 3) ذخیره مقدار در HttpContext
                if (!string.IsNullOrWhiteSpace(upn))
                    context.Items["fundCode"] = upn;
            }

            await _next(context);
        }
    }
}
