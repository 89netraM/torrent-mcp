FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       clang zlib1g-dev

WORKDIR /source

COPY TorrentMcp.slnx Directory.Build.props .editorconfig ./
COPY TorrentMcp.Domain/TorrentMcp.Domain.csproj TorrentMcp.Domain/packages.lock.json ./TorrentMcp.Domain/
COPY TorrentMcp.FloodIntegration/TorrentMcp.FloodIntegration.csproj TorrentMcp.FloodIntegration/packages.lock.json ./TorrentMcp.FloodIntegration/
COPY TorrentMcp.Server/TorrentMcp.Server.csproj TorrentMcp.Server/packages.lock.json ./TorrentMcp.Server/
RUN dotnet restore --locked-mode

COPY TorrentMcp.Domain/ ./TorrentMcp.Domain/
COPY TorrentMcp.FloodIntegration/ ./TorrentMcp.FloodIntegration/
COPY TorrentMcp.Server/ ./TorrentMcp.Server/
RUN dotnet publish --no-restore --output /app ./TorrentMcp.Server/TorrentMcp.Server.csproj

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["/app/TorrentMcp.Server"]
