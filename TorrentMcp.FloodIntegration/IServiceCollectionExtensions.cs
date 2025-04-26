using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TorrentMcp.Domain.Services;

namespace TorrentMcp.FloodIntegration;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddFloodClient(this IServiceCollection services)
    {
        services
            .AddSingleton<IValidateOptions<FloodClientOptions>, ValidateFloodClientOptions>()
            .AddOptions<FloodClientOptions>()
            .BindConfiguration("FloodClient")
            .ValidateOnStart();

        services.AddMemoryCache();

        services.AddHttpClient<FloodAuthDelegatingHandler>(
            (sp, client) =>
            {
                var floodOptions = sp.GetRequiredService<IOptions<FloodClientOptions>>().Value;
                client.BaseAddress = floodOptions.BaseUrl;
            }
        );

        services
            .AddHttpClient<ITorrentClient, FloodClient>(
                (sp, client) =>
                {
                    var floodOptions = sp.GetRequiredService<IOptions<FloodClientOptions>>().Value;
                    client.BaseAddress = floodOptions.BaseUrl;
                }
            )
            .AddHttpMessageHandler<FloodAuthDelegatingHandler>();

        return services;
    }
}
