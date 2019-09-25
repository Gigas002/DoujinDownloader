#Win-x64
dotnet publish -c Release -r win-x64 -o Publish\win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained

#Win-x86
dotnet publish -c Release -r win-x86 -o Publish\win-x86 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained

#osx-x64
dotnet publish -c Release -r osx-x64 -o Publish\osx-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained

#linux-x64
dotnet publish -c Release -r linux-x64 -o Publish\linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained