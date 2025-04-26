using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TorrentMcp.Domain.Entities;
using TorrentMcp.Domain.Services;
using TorrentMcp.FloodIntegration.Dtos;

namespace TorrentMcp.FloodIntegration;

internal class FloodClient(ILogger<FloodClient> logger, HttpClient httpClient) : ITorrentClient
{
    public async Task<Result<TorrentItem[]>> ListTorrentsAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("torrents", cancellationToken);

        if (await TryGetErrorStatusCode<TorrentItem[]>(response, cancellationToken) is { } errorResult)
        {
            return errorResult;
        }

        var torrentListSummary = await response.Content.ReadFromJsonAsync(
            FloodJsonSerializerContext.Default.TorrentListSummary,
            cancellationToken
        );
        if (torrentListSummary is null)
        {
            logger.LogWarning("Read `null` when parsing torrents list");
            return new Result<TorrentItem[]>.Error();
        }

        return new Result<TorrentItem[]>.Ok(
            [
                .. torrentListSummary.Torrents.Select(kvp => new TorrentItem(
                    new(kvp.Key),
                    kvp.Value.Name,
                    kvp.Value.Directory,
                    new((long)kvp.Value.BytesDone, (long)kvp.Value.SizeBytes)
                )),
            ]
        );
    }

    public async Task<Result<TorrentId>> AddTorrentByFileAsync(
        Stream torrentFile,
        string directory,
        CancellationToken cancellationToken
    )
    {
        var torrentFileBase64 = await StreamToBase64String(torrentFile, cancellationToken);

        var response = await httpClient.PostAsJsonAsync(
            "torrents/add-files",
            new() { Files = [torrentFileBase64], Destination = directory },
            FloodJsonSerializerContext.Default.AddTorrentsBytFileOptions,
            cancellationToken
        );

        if (response.StatusCode is HttpStatusCode.Forbidden)
        {
            return new Result<TorrentId>.Error($"Forbidden directory: {directory}");
        }
        if (await TryGetErrorStatusCode<TorrentId>(response, cancellationToken) is { } errorResult)
        {
            return errorResult;
        }

        var addedTorrentIds = await response.Content.ReadFromJsonAsync(
            FloodJsonSerializerContext.Default.AddTorrentResponse,
            cancellationToken
        );
        if (addedTorrentIds is not [string id])
        {
            logger.LogWarning(
                "Expected to receive a single entry list of hashes when adding a torrent file, but received {@AddedTorrentIds} ({StatusCode})",
                addedTorrentIds,
                response.StatusCode
            );
            return new Result<TorrentId>.Error();
        }

        return new Result<TorrentId>.Ok(new(id));

        static async Task<string> StreamToBase64String(Stream stream, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream((int)stream.Length);
            await stream.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            if (!ms.TryGetBuffer(out var buffer) || buffer.Array is null)
            {
                throw new Exception();
            }
            return Convert.ToBase64String(buffer.Array, buffer.Offset, buffer.Count);
        }
    }

    public async Task<Result<TorrentId>> RemoveTorrentAsync(TorrentId torrentId, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "torrents/delete",
            new() { Hashes = [torrentId.Value] },
            FloodJsonSerializerContext.Default.DeleteTorrentsOptions,
            cancellationToken
        );

        if (await TryGetErrorStatusCode<TorrentId>(response, cancellationToken) is { } errorResult)
        {
            return errorResult;
        }

        return new Result<TorrentId>.Ok(torrentId);
    }

    private async Task<Result<T>?> TryGetErrorStatusCode<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    )
    {
        if (response.StatusCode is HttpStatusCode.InternalServerError)
        {
            var error = await response.Content.ReadFromJsonAsync(
                FloodJsonSerializerContext.Default.Error,
                cancellationToken
            );
            logger.LogWarning("Received internal server error from Flood, {ErrorMessage}", error?.Message);
            return new Result<T>.Error(error?.Message);
        }
        response.EnsureSuccessStatusCode();

        return null;
    }
}
