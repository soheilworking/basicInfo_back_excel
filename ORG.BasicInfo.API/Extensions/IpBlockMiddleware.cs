public class IpBlockMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRedisBlockedIpStore _store;
    private readonly ILogger<IpBlockMiddleware> _logger;

    public IpBlockMiddleware(RequestDelegate next, IRedisBlockedIpStore store, ILogger<IpBlockMiddleware> logger)
    {
        _next = next;
        _store = store;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        if (!string.IsNullOrEmpty(ip))
        {
            if (await _store.IsBlockedAsync(ip, context.RequestAborted))
            {
                _logger.LogWarning("Blocked request from IP {Ip}", ip);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden", context.RequestAborted);
                }
                else
                {
                    _logger.LogError("Response already started. Cannot write 403 for IP {Ip}", ip);
                    context.Abort();
                }

                return;
            }
        }

        await _next(context);
    }
}
