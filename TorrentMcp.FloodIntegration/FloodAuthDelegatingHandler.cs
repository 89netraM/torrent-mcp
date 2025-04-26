using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TorrentMcp.FloodIntegration.Dtos;

namespace TorrentMcp.FloodIntegration;

internal class FloodAuthDelegatingHandler(
    IOptions<FloodClientOptions> options,
    HttpClient httpClient,
    IMemoryCache memoryCache
) : DelegatingHandler
{
    private static readonly object FloodClientJwtCacheKey = new();

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        request.Headers.Add("Cookie", $"jwt={await FetchJwt(cancellationToken)}");

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> FetchJwt(CancellationToken cancellationToken) =>
        await memoryCache.GetOrCreateAsync(
            FloodClientJwtCacheKey,
            async entry =>
            {
                var response = await httpClient.PostAsJsonAsync(
                    "auth/authenticate",
                    new() { Username = options.Value.Username, Password = options.Value.Password },
                    FloodJsonSerializerContext.Default.AuthAuthenticationOptions,
                    cancellationToken
                );

                var (jwt, expiresAt) = GetJwt(response.Headers);
                entry.AbsoluteExpiration = expiresAt;

                return jwt;
            }
        ) ?? throw new NullJwtException();

    private (string jwt, DateTimeOffset expiresAt) GetJwt(HttpResponseHeaders headers)
    {
        var cookies = GetCookies(headers);
        if (cookies["jwt"] is not { Value: var jwt, Expires: var expiresAt })
        {
            throw new NoJwtReceivedException();
        }

        return (jwt, (DateTimeOffset)expiresAt);
    }

    private CookieCollection GetCookies(HttpResponseHeaders headers)
    {
        var cookieContainer = new CookieContainer();

        foreach (var cookieHeader in headers.GetValues("Set-Cookie"))
        {
            cookieContainer.SetCookies(options.Value.BaseUrl, cookieHeader);
        }

        return cookieContainer.GetCookies(options.Value.BaseUrl);
    }
}

file class NoJwtReceivedException() : Exception("No JWT received from authentication endpoint");

file class NullJwtException() : Exception("Received `null` from cache when fetching Flood JWT");
