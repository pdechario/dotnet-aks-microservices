using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace DotnetAksMicroservices.Platform.Common.Middleware;

public class StructuredLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public StructuredLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        {
            await _next(context);
        }
    }
}
