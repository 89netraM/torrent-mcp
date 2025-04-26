using System.Text.Json.Serialization;

namespace TorrentMcp.FloodIntegration.Dtos;

public class AuthAuthenticationOptions
{
    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("password")]
    public required string Password { get; set; }
}
