using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TorrentMcp.Server.Dtos;

[JsonSerializable(typeof(AddTorrentResponse))]
[JsonSerializable(typeof(DeleteTorrentResponse))]
[JsonSerializable(typeof(IEnumerable<TorrentItem>))]
[JsonSerializable(typeof(IFormFile))]
[JsonSerializable(typeof(ProblemDetails))]
public partial class TorrentMcpJsonSerializerContext : JsonSerializerContext;
