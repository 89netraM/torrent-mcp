using System.Text.Json.Serialization;

namespace TorrentMcp.FloodIntegration.Dtos;

internal class AddTorrentsBytFileOptions
{
    [JsonPropertyName("files")]
    public required string[] Files { get; set; }

    [JsonPropertyName("destination")]
    public required string Destination { get; set; }

    [JsonPropertyName("tags")]
    public string[] Tags { get; set; } = [];

    [JsonPropertyName("isBasePath")]
    public bool IsBasePath { get; set; } = true;

    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; set; } = true;

    [JsonPropertyName("isSequential")]
    public bool IsSequential { get; set; } = true;

    [JsonPropertyName("isInitialSeeding")]
    public bool IsInitialSeeding { get; set; } = true;

    [JsonPropertyName("start")]
    public bool Start { get; set; } = true;
}
