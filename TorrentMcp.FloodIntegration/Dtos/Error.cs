using System.Text.Json.Serialization;

namespace TorrentMcp.FloodIntegration.Dtos;

internal class Error
{
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
