# Docker create image

# dotnet publish console app
dotnet publish "DoujinDownloader/DoujinDownloader.csproj" -c Release /p:Platform=x64

# docker build
docker build -t doujindownloader -f Dockerfile .

# check image
# docker images
# docker run -it --rm doujindownloader