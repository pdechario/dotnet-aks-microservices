using Microsoft.Extensions.DependencyInjection;

namespace DotnetAksMicroservices.Platform.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        return services;
    }
}
