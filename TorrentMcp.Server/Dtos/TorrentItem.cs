namespace TorrentMcp.Server.Dtos;

public class TorrentItem
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Directory { get; set; }
    public long? DownloadedBytes { get; set; }
    public long? TotalBytes { get; set; }
}
