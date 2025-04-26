using System.Text.Json.Serialization;

namespace TorrentMcp.FloodIntegration.Dtos;

public class DeleteTorrentsOptions
{
    [JsonPropertyName("hashes")]
    public required string[] Hashes { get; set; }

    [JsonPropertyName("deleteData")]
    public bool DeleteData { get; set; } = false;
}
