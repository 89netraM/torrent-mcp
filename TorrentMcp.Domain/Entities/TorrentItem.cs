namespace TorrentMcp.Domain.Entities;

public record TorrentItem(TorrentId Id, string Name, string Directory, ProgressReport ProgressReport);
