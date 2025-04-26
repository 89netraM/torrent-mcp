using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TorrentMcp.FloodIntegration;
using TorrentMcp.Server;
using TorrentMcp.Server.Dtos;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, TorrentMcpJsonSerializerContext.Default);
});

builder.Services.AddOutputCache();

builder.Services.AddFloodClient();

builder
    .Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<TorrentTools>(new() { TypeInfoResolverChain = { TorrentMcpJsonSerializerContext.Default } });

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapOpenApi().CacheOutput().RequireAuthorization();

app.MapTorrentRestEndpoints().RequireAuthorization();

app.MapMcp().RequireAuthorization();

app.Run();
