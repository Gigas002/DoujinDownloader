# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /DoujinDownloader

# copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore DoujinDownloader/DoujinDownloader.csproj -r linux-x64

# copy and publish app and libraries
RUN dotnet publish DoujinDownloader/DoujinDownloader.csproj -c Release -o /app -r linux-x64 --self-contained false --no-restore
# Default
# RUN dotnet publish DoujinDownloader/DoujinDownloader.csproj -c release -o /app /p:Platform=x64
# Slim
# RUN dotnet publish DoujinDownloader/DoujinDownloader.csproj -c Release -o /app -r linux-x64 --self-contained true --no-restore /p:PublishTrimmed=true /p:PublishReadyToRun=true

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:6.0-focal
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "DoujinDownloader.dll"]