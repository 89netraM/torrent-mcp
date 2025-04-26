using System.Text.Json.Serialization;

namespace TorrentMcp.FloodIntegration.Dtos;

internal class TorrentProperties
{
    [JsonPropertyName("bytesDone")]
    public required double BytesDone { get; set; }

    [JsonPropertyName("directory")]
    public required string Directory { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("sizeBytes")]
    public required double SizeBytes { get; set; }
}
