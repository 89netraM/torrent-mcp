// using System.Threading;
// using System.Threading.Tasks;
// using ModelContextProtocol.Protocol.Types;
// using ModelContextProtocol.Server;

// namespace TorrentMcp.Server.McpTools;

// public class DeleteTorrentTool : McpServerTool
// {
//     public override Tool ProtocolTool { get; } = new()
//     {
//         Name = "Delete torrent",
//         Description = "Deletes a single torrent without removing the downloaded data.",
//         Annotations = new()
//         {
//             DestructiveHint = true,
//             IdempotentHint = false,
//             OpenWorldHint = false,
//             ReadOnlyHint = false,
//             Title = "Delete torrent",
//         },
//     };

//     public override ValueTask<CallToolResponse> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
//     {
//         throw new System.NotImplementedException();
//     }
// }
