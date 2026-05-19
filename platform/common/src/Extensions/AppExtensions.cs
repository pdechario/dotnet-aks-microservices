using Common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Common.Extensions;

public static class AppExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UseStructuredLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<StructuredLoggingMiddleware>();
    }
}
