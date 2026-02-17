namespace ORG.BasicInfo.API.Extensions
{
    public static class JwtSessionValidationExtensions
    {
        public static IApplicationBuilder UseJwtSessionValidation(this IApplicationBuilder app, string sessionCookieName = "session_id")
        {
            using var scope = app.ApplicationServices.CreateScope();
            var redis = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();
            return app.UseMiddleware<JwtSessionValidationMiddleware>(redis,  sessionCookieName);
        }
    }

}
