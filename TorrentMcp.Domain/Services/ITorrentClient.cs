using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TorrentMcp.Domain.Entities;

namespace TorrentMcp.Domain.Services;

public interface ITorrentClient
{
    Task<Result<TorrentItem[]>> ListTorrentsAsync(CancellationToken cancellationToken);

    Task<Result<TorrentId>> AddTorrentByFileAsync(
        Stream torrentFile,
        string directory,
        CancellationToken cancellationToken
    );

    Task<Result<TorrentId>> RemoveTorrentAsync(TorrentId torrentId, CancellationToken cancellationToken);
}

public abstract record Result<T>
{
    public record Ok(T Value) : Result<T>;

    public record Error(string? Message = null) : Result<T>;
}
