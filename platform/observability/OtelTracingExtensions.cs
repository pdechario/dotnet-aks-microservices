using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Platform.Observability;

public static class OtelTracingExtensions
{
    public static readonly ActivitySource ActivitySource = new("Platform.Observability");

    public static IServiceCollection AddPlatformTracing(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(serviceName: serviceName)
                    .AddEnvironmentVariableDetector())
                .WithTracing(tracing => tracing
                    .AddSource(ActivitySource.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter());
            
            return services;
        }
}