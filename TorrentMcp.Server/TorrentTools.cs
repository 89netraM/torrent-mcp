using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;
using TorrentMcp.Domain.Entities;
using TorrentMcp.Domain.Services;
using TorrentMcp.Server.Dtos;
using TorrentItemDto = TorrentMcp.Server.Dtos.TorrentItem;
using TorrentItemEntity = TorrentMcp.Domain.Entities.TorrentItem;

namespace TorrentMcp.Server;

[McpServerToolType]
public class TorrentTools(ITorrentClient torrentClient)
{
    [McpServerTool]
    [Description("Lists all currently downloading torrents.")]
    private async Task<IResult> GetTorrents(CancellationToken cancellationToken) =>
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

    [McpServerTool]
    [Description("Starts downloading a new torrent from a file into a directory.")]
    private async Task<IResult> StartTorrent(
        string directory,
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

    [McpServerTool]
    [Description("Stops downloading a torrent specified with it's ID.")]
    private async Task<IResult> StopTorrent(string id, CancellationToken cancellationToken) =>
        await torrentClient.RemoveTorrentAsync(new(id), cancellationToken) switch
        {
            Result<TorrentId>.Ok => Results.Ok(new DeleteTorrentResponse { TorrentId = id }),
            Result<TorrentId>.Error(var errorMessage) => Results.InternalServerError(
                new ProblemDetails() { Title = errorMessage }
            ),
            _ => Results.InternalServerError(new ProblemDetails()),
        };
}
