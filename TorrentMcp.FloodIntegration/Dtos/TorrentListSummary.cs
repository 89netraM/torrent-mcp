using System.Text.Json.Serialization;

namespace TorrentMcp.FloodIntegration.Dtos;

internal class TorrentListSummary
{
    [JsonPropertyName("torrents")]
    public required TorrentList Torrents { get; set; }
}
