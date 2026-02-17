using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ORG.BasicInfo.Data;
using Mapster;
using Ardalis.Result;
using FluentValidation.Results;
using RestSharp;
using StackExchange.Redis;
using System.Threading.RateLimiting;
using ClassEncryptionLibrary;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Cryptography.X509Certificates;
//using Microsoft.AspNetCore.Authentication.Certificate;
namespace ORG.BasicInfo.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{

    private static readonly Assembly AssemblyToScan = typeof(IFeatureMarker).Assembly;
    private static SqlConnection _connection;

    public static void ConfigureJwtSchemes(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetRequiredSection("JwtAuth")
            .Get<JwtOptions>(o => o.BindNonPublicProperties = true);

        var accessOptions = configuration.GetRequiredSection("JwtAcceess")
            .Get<JwtOptions>(o => o.BindNonPublicProperties = true);
        services.Configure<JwtOptions>("JwtAuth", configuration.GetSection("JwtAuth")); 
        services.Configure<JwtOptions>("JwtAccessScheme", configuration.GetSection("JwtAcceess"));
        // ----------------------------------------------------
        // 1) AddAuthentication (only once)
        // ----------------------------------------------------
        services.AddAuthentication(options =>
        {
            // هیچ default scheme ای نمی‌ذاریم
            // تا هر کنترلر خودش انتخاب کند
        })

        // ----------------------------------------------------
        // 2) Certificate Authentication
        // ----------------------------------------------------
        .AddCertificate("CertScheme", options =>
        {
            options.AllowedCertificateTypes = CertificateTypes.All;

            options.Events = new CertificateAuthenticationEvents
            {
                OnCertificateValidated = async context =>
                {
                    var http = context.HttpContext;
                    var cert = context.ClientCertificate;

                    // Validate CA
                    var rootCa = new X509Certificate2("./certificate/rootCA.pfx", "361SoheiL@");

                    using var chain = new X509Chain();
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

                    bool isValidChain = chain.Build(cert);
                    bool issuedByMyCA = chain.ChainElements
                        .Cast<X509ChainElement>()
                        .Any(x => x.Certificate.Thumbprint == rootCa.Thumbprint);

                    if (!isValidChain || !issuedByMyCA)
                    {
                        context.Fail("Certificate is not issued by trusted CA");
                        return;
                    }

                    // مسیر خاص
                    var path = http.Request.Path.Value;
                    var method = http.Request.Method;

                    if (path.Equals("/api/auth", StringComparison.OrdinalIgnoreCase) &&
                        method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                    {
                        http.Request.EnableBuffering();

                        string bodyStr;
                        using (var reader = new StreamReader(http.Request.Body, Encoding.UTF8, leaveOpen: true))
                        {
                            bodyStr = await reader.ReadToEndAsync();
                            http.Request.Body.Position = 0;
                        }

                        var json = System.Text.Json.JsonDocument.Parse(bodyStr);

                        if (!json.RootElement.TryGetProperty("fundCode", out var fundCodeElement))
                        {
                            context.Fail("fundCode is missing");
                            return;
                        }

                        context.Success();
                        return;
                    }

                    context.Success();
                }
            };
        })

        // ----------------------------------------------------
        // 3) JWT Refresh Token Scheme
        // ----------------------------------------------------
        .AddJwtBearer("JwtAuth", options =>
        {
            var key = Encoding.UTF8.GetBytes(authOptions.SigningKey);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = authOptions.Issuer,
                ValidAudience = authOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    if (ctx.Request.Cookies.TryGetValue("referesh_token", out var token))
                        ctx.Token = token;

                    return Task.CompletedTask;
                }
            };
        })

        // ----------------------------------------------------
        // 4) JWT Access Token Scheme
        // ----------------------------------------------------
        .AddJwtBearer("JwtAccessScheme", options =>
        {
            var key = Encoding.UTF8.GetBytes(accessOptions.SigningKey);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = accessOptions.Issuer,
                ValidAudience = accessOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    if (ctx.Request.Cookies.TryGetValue("access_token", out var token))
                        ctx.Token = token;

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();
    }
    public static void ConfigureRedisDB(this IServiceCollection services, IConfiguration configuration)
    {
        // خواندن تنظیمات
        var redisConfig = configuration.GetSection("Redis").GetValue<string>("Configuration");
        var instanceName = configuration.GetSection("Redis").GetValue<string>("InstanceName");

        // 1) ثبت ConnectionMultiplexer به‌صورت Singleton (توصیه‌شده)
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisConfig);
        });

        // 2) (اختیاری) استفاده از IDistributedCache مبتنی بر StackExchange.Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig;
            options.InstanceName = instanceName;
        });

        // 3) ثبت یک wrapper ساده برای دسترسی مستقیم به IDatabase
        services.AddScoped(sp =>
        {
            var mux = sp.GetRequiredService<IConnectionMultiplexer>();
            return mux.GetDatabase();
        });
        services.AddScoped<IRedisCacheService, RedisCacheService>();
    }

    public static void AddBasicInfoDbContext(this IServiceCollection services,string ConnectionString)
    {

        services.AddSingleton<ExternalDbSingletonService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connStr = config.GetConnectionString("ExternalDb");
            return new ExternalDbSingletonService(connStr);
        });

        SqlConnection connection;
        connection = new SqlConnection(ConnectionString);
        connection.Open();
   

        services.AddDbContext<FormsInfoDbContext>((serviceProvider, optionsBuilder) =>
            optionsBuilder.UseSqlServer(ConnectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(120);  // 120 ثانیه برای اجرای هر کوئری
                                                 // در صورت نیاز به Retry On Failure:
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(120), null);
            }),ServiceLifetime.Scoped);

        //using var _connectionInitial = new SqlConnection(ConnectionString);
        //_connectionInitial.Open();
        var optionsBuilder = new DbContextOptionsBuilder<FormsInfoDbContext>();
        optionsBuilder.UseSqlServer(connection);
        using var _dbContexts = new FormsInfoDbContext(optionsBuilder.Options);
        //InitialData initialData = new InitialData(_dbContexts);
        //bool result = false;
        //result = initialData.AddedData();
        //if (result == false)
        //    Console.WriteLine("not added data in the first run service.");

    }
    public async static Task<IHost> MigrateAndSeed(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<FormsInfoDbContext>();

        // 1. Apply any pending migrations
        await context.Database.MigrateAsync();


        return host;
    }
    public static void AddFeatures(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

    
        config.Scan(AssemblyToScan);
 

        config.Default.RequireDestinationMemberSource(false);
        config.Default.MapToConstructor(true);
        //config.Default.NameMatchingStrategy(NameMatchingStrategy.Exact);
        // 1) map each FluentValidation.ValidationFailure → Ardalis.ValidationError
        TypeAdapterConfig<FluentValidation.Results.ValidationFailure, ValidationError>
          .NewConfig()
          .ConstructUsing(src =>
            new ValidationError(src.PropertyName, src.ErrorMessage));

        // 2) map the whole ValidationResult → List<Ardalis.ValidationError>
        TypeAdapterConfig<ValidationResult, List<ValidationError>>
          .NewConfig()
          .ConstructUsing(src =>
            src.Errors
               .Select(x => x.Adapt<ValidationError>())
               .ToList());

        config.ForType<ulong, Guid>();
        config.ForType<ulong[], Guid[]>();
        //Note
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
              builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
   
        var  _aesEncryptionSessions = new AesEncryption("7ec4231a048b7382dced655a1d5d7200d042cb10714957f0061d5b3e506aace4", "745ed9359f603865391a81b3f601e07e");

        services.AddSingleton(_aesEncryptionSessions);
        services.AddScoped<DbContext, FormsInfoDbContext>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyToScan));
        services.AddValidatorsFromAssembly(AssemblyToScan);
    }
    public static void AddRateLimit(this IServiceCollection services)
    {
        // ثبت RedisBlockedIpStore قبلاً انجام شده باشد
        services.AddSingleton<IRedisBlockedIpStore, RedisBlockedIpStore>();

        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetTokenBucketLimiter(ip, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                    TokensPerPeriod = 10
                });
            });

            // وقتی ریجکت شد، OnRejected فراخوانی می‌شود
            options.OnRejected = async (context, cancellationToken) =>
            {
                // وضعیت پاسخ
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                try
                {
                    // سعی کنید از DI یک store بگیرید و آی‌پی را بلاک کنید
                    var store = context.HttpContext.RequestServices.GetService<IRedisBlockedIpStore>();
                    var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
                    if (!string.IsNullOrEmpty(ip) && store != null)
                    {
                        // بلاک موقت مثلاً 60 ثانیه؛ مقدار را براساس نیاز تنظیم کنید
                        await store.BlockIpAsync(ip, TimeSpan.FromSeconds(60*5), cancellationToken).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // اگر نوشتن به Redis شکست خورد، لاگ بزنید اما کاربر را ریجکت کنید
                }

                // پیام اختیاری به کلاینت
                await context.HttpContext.Response.WriteAsync("Too many requests", cancellationToken).ConfigureAwait(false);
            };
        });

    }
}
