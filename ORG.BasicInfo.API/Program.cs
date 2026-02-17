using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using ORG.BasicInfo.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Scalar.AspNetCore;
using ORG.BasicInfo.Data;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

var options = new WebApplicationOptions
{
    Args = args,
    WebRootPath = "public",
    ContentRootPath = AppContext.BaseDirectory
};

var builder = WebApplication.CreateBuilder(options);

WebApplicationBuilder builder1 = builder;

builder1.Services.Configure<KestrelServerOptions>(o => o.AddServerHeader = false);
builder1.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);
builder1.Services.Configure<ApiBehaviorOptions>(o => o.SuppressModelStateInvalidFilter = true);
builder1.Services.Configure<JsonOptions>(json =>
{
    json.JsonSerializerOptions.WriteIndented = false;
    json.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    json.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    json.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    json.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    json.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
builder1.Services.AddHttpContextAccessor();
builder1.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder1.Services.AddResponseCompression();
builder1.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = ApiVersion.Default;
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
})
.AddApiExplorer(o =>
{
    o.GroupNameFormat = "'v'VVV";
    o.SubstituteApiVersionInUrl = true;
});
builder1.Host.UseDefaultServiceProvider((context, opts) =>
{
    opts.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    opts.ValidateOnBuild = true;
}).UseWindowsService();
builder1.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(https =>
    {
        https.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
        
        https.ClientCertificateValidation = (cert, chain, errors) =>
        {
            return true;
            if (cert == null)
                return false;
           
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                bool isValid = chain.Build(cert); // اگر می‌خواهی فقط RootCA خودت را قبول کند:
                var rootCa = new X509Certificate2("./certificate/rootCA.pfx", "361SoheiL@");
                bool issuedByMyCA = chain.ChainElements
                    .Cast<X509ChainElement>()
                    .Any(x => x.Certificate.Thumbprint == rootCa.Thumbprint);
            return false;
                return isValid && issuedByMyCA;
            };


    });
    });
builder.WebHost.UseKestrel();
builder1.Services.ConfigureJwtSchemes(builder1.Configuration);
builder1.Services.ConfigureRedisDB(builder1.Configuration);
builder1.Services.AddOpenApi();
builder1.Services.AddProblemDetails();
builder1.Services.AddControllers();
string conn = builder1.Configuration.GetRequiredSection("ConnectionStrings__DefaultConnection")
    .Get<string>(o => o.BindNonPublicProperties = true);

if (string.IsNullOrEmpty(conn))
    throw new InvalidOperationException("DefaultConnection is missing!");
builder1.Services.AddBasicInfoDbContext(conn);
builder1.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new RemoveControllerSuffixTransformer()));
});
builder1.Services.AddRateLimit();
builder1.Services.AddFeatures();
builder1.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy
            .WithOrigins(
                "https://10.0.11.21:7113",
                "https://localhost:7113",
                 //"https://10.0.52.130:7113",
                "http://localhost:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("X-Status-Text")
    );
});
// -------------------- BUILD --------------------
//builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressMapClientErrors = true; });

var app = builder1.Build();

if (app.Environment.IsDevelopment())
    app.MapScalarApiReference();
// -------------------- PIPELINE (اصلاح‌شده) --------------------

// ✅ باید قبل از هر چیز که ممکن است Response را شروع کند
app.UseForwardedHeaders();
app.UseHttpsRedirection();
// ✅ Compression قبل از MVC ولی بعد از ForwardedHeaders
app.UseResponseCompression();

// ✅ CORS قبل از Auth
app.UseCors("CorsPolicy");

// ✅ Authentication → Authorization
//app.UseMiddleware<CertificateFundCodeMiddleware>();
app.Use(async (context, next) => 
{ 
    var endpoint = context.GetEndpoint(); 
    var hasAttr = endpoint?.Metadata.GetMetadata<UseCertificateFundCodeAttribute>() != null; 
    if (hasAttr) { 
        var middleware = new CertificateFundCodeMiddleware(next); 
        await middleware.Invoke(context); 
    } else { await next(context); } 
});

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/test-cert", (HttpContext context) => { 
    var cert = context.Connection.ClientCertificate; 
    if (cert == null) 
        return Results.Text("NO CERT RECEIVED"); 

    return Results.Text($"CERT RECEIVED: {cert.Subject}"); 
});
// ✅ این دو باید قبل از Session و قبل از MVC باشند
app.UseMiddleware<IpBlockMiddleware>();
app.UseJwtSessionValidation("sessionId");

// ✅ Rate Limiter بعد از Auth و قبل از MVC
app.UseRateLimiter();

// ✅ Session بعد از JwtSessionValidation
app.UseSession();

// ✅ Static files
app.UseDefaultFiles();
app.UseStaticFiles();

// ✅ OpenAPI
app.MapOpenApi();

// ✅ MVC
app.MapControllers() ;

app.Run();
