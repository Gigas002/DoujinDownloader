# Select current dotnet version
FROM mcr.microsoft.com/dotnet/core/runtime:3.1

# copy from to
COPY DoujinDownloader/bin/x64/Release/netcoreapp3.1/publish DoujinDownloader/

ENTRYPOINT ["dotnet", "DoujinDownloader/DoujinDownloader.dll"]