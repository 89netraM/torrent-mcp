using System.Text.Json.Serialization;

namespace TorrentMcp.FloodIntegration.Dtos;

[JsonSerializable(typeof(AddTorrentsBytFileOptions))]
[JsonSerializable(typeof(AddTorrentResponse))]
[JsonSerializable(typeof(AuthAuthenticationOptions))]
[JsonSerializable(typeof(DeleteTorrentsOptions))]
[JsonSerializable(typeof(Error))]
[JsonSerializable(typeof(TorrentList))]
[JsonSerializable(typeof(TorrentListSummary))]
[JsonSerializable(typeof(TorrentProperties))]
[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNameCaseInsensitive = true,
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true
)]
internal partial class FloodJsonSerializerContext : JsonSerializerContext;
