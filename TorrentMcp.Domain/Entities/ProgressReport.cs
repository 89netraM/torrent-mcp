namespace TorrentMcp.Domain.Entities;

public record ProgressReport(long DownloadedBytes, long TotalBytes);
