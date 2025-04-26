using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TorrentMcp.Domain.Entities;
using TorrentMcp.Domain.Services;
using TorrentMcp.Server.Dtos;
using TorrentItemDto = TorrentMcp.Server.Dtos.TorrentItem;
using TorrentItemEntity = TorrentMcp.Domain.Entities.TorrentItem;

namespace TorrentMcp.Server;

public static class TorrentRestEndpoints
{
    public static RouteGroupBuilder MapTorrentRestEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/");

        group
            .MapGet("/torrents", GetTorrents)
            .Produces<IEnumerable<TorrentItemDto>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group
            .MapPost("/torrents", PostTorrent)
            .Produces<AddTorrentResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group
            .MapDelete("/torrents/{id}", DeleteTorrent)
            .Produces<DeleteTorrentResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetTorrents(
        [FromServices] ITorrentClient torrentClient,
        CancellationToken cancellationToken
    ) =>
        await torrentClient.ListTorrentsAsync(cancellationToken) switch
        {
            Result<TorrentItemEntity[]>.Ok(var items) => Results.Ok(
                items.Select(i => new TorrentItemDto
                {
                    Id = i.Id.Value,
                    Name = i.Name,
                    Directory = i.Directory,
                    DownloadedBytes = i.ProgressReport.DownloadedBytes,
                    TotalBytes = i.ProgressReport.TotalBytes,
                })
            ),
            Result<TorrentItemEntity[]>.Error(var errorMessage) => Results.InternalServerError(
                new ProblemDetails() { Title = errorMessage }
            ),
            _ => Results.InternalServerError(new ProblemDetails()),
        };

    private static async Task<IResult> PostTorrent(
        [FromServices] ITorrentClient torrentClient,
        [FromQuery] string directory,
        IFormFile torrentFile,
        CancellationToken cancellationToken
    ) =>
        await torrentClient.AddTorrentByFileAsync(torrentFile.OpenReadStream(), directory, cancellationToken) switch
        {
            Result<TorrentId>.Ok(var id) => Results.Ok(new AddTorrentResponse { TorrentId = id.Value }),
            Result<TorrentId>.Error(var errorMessage) => Results.InternalServerError(
                new ProblemDetails() { Title = errorMessage }
            ),
            _ => Results.InternalServerError(new ProblemDetails()),
        };

    private static async Task<IResult> DeleteTorrent(
        [FromServices] ITorrentClient torrentClient,
        [FromRoute] string id,
        CancellationToken cancellationToken
    ) =>
        await torrentClient.RemoveTorrentAsync(new(id), cancellationToken) switch
        {
            Result<TorrentId>.Ok => Results.Ok(new DeleteTorrentResponse { TorrentId = id }),
            Result<TorrentId>.Error(var errorMessage) => Results.InternalServerError(
                new ProblemDetails() { Title = errorMessage }
            ),
            _ => Results.InternalServerError(new ProblemDetails()),
        };
}
