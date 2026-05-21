using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Observability.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Observability.Extensions;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetSection("Otel").Get<OtelOptions>()
            ?? throw new InvalidOperationException(
                "Missing required configuration section 'Otel'. " +
                "Ensure 'Otel:ServiceName' and 'Otel:Endpoint' are set.");

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(options.ServiceName))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(options.Endpoint);
                }))
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(options.Endpoint);
                }))
            .WithLogging(logging => logging
                .AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(options.Endpoint);
                }));
        return services;
    }
}
