$buildAll=$args[0]

Write-Output "Started building/publishing"

# win-x64
if ($IsWindows -or $buildAll)
{
    Write-Output "Start Win-x64 publish"
    dotnet publish "DoujinDownloader/DoujinDownloader.csproj" -c Release -r win-x64 --self-contained true /p:PublishDir=../Publish/win-x64 /p:IncludeAllContentForSelfExtract=true /p:PublishSingleFile=true /p:PublishTrimmed=true
    Write-Output "Ended Win-x64 publish"
}

# linux-x64
if ($IsLinux -or $buildAll)
{
    Write-Output "Start Linux-x64-Benchmarks publish"
    dotnet publish "DoujinDownloader/DoujinDownloader.csproj" -c Release -r linux-x64 /p:PublishDir=../Publish/linux-x64 /p:IncludeAllContentForSelfExtract=true /p:PublishSingleFile=true /p:PublishTrimmed=true
    Write-Output "Ended Linux-x64-Benchmarks publish"
}

# osx-x64
if ($IsMacOS -or $buildAll)
{
    Write-Output "Start Osx-x64-Benchmarks publish"
    dotnet publish "DoujinDownloader/DoujinDownloader.csproj" -c Release -r osx-x64 /p:PublishDir=../Publish/osx-x64 /p:IncludeAllContentForSelfExtract=true /p:PublishSingleFile=true /p:PublishTrimmed=true
    Write-Output "Ended Osx-x64-Benchmarks publish"
}

Write-Output "Ended building/publishing"

# Remove all *.pdb files
Write-Output "Removing all *.pdb files from Publish directory"
Get-ChildItem "Publish/" -Include *.pdb -Recurse | Remove-Item

# Copy docs, etc to published directories before zipping them
Write-Output "Copying docs, LICENSE.md, etc to published directories"

# win-x64
if ($IsWindows -or $buildAll)
{
    Copy-Item -Path "README.md" -Destination "Publish/win-x64/README.pdf"
    Copy-Item -Path "LICENSE.md" -Destination "Publish/win-x64/LICENSE.md"
    Copy-Item -Path "CHANGELOG.md" -Destination "Publish/win-x64/CHANGELOG.md"    
}

# linux-x64
if ($IsLinux -or $buildAll)
{
    Copy-Item -Path "README.md" -Destination "Publish/linux-x64/README.pdf"
    Copy-Item -Path "LICENSE.md" -Destination "Publish/linux-x64/LICENSE.md"
    Copy-Item -Path "CHANGELOG.md" -Destination "Publish/linux-x64/CHANGELOG.md"    
}

# osx-x64
if ($IsMacOS -or $buildAll)
{
    Copy-Item -Path "README.md" -Destination "Publish/osx-x64/README.pdf"
    Copy-Item -Path "LICENSE.md" -Destination "Publish/osx-x64/LICENSE.md"
    Copy-Item -Path "CHANGELOG.md" -Destination "Publish/osx-x64/CHANGELOG.md"
}

# Add everything into archives. Requires installed 7z added to PATH.
Write-Output "Started adding published files to .zip archives"

# win-x64
if ($IsWindows -or $buildAll)
{
    Write-Output "Start adding win-x64 to .zip"
    Start-Process -NoNewWindow -Wait -FilePath "7z" -ArgumentList "a ../win-x64.zip *" -WorkingDirectory "Publish/win-x64/"
    Write-Output "Ended adding win-x64 to .zip"
}

# linux-x64
if ($IsLinux -or $buildAll)
{
    Write-Output "Start adding linux-x64 to .zip"
    Start-Process -NoNewWindow -Wait -FilePath "7z" -ArgumentList "a ../linux-x64.zip *" -WorkingDirectory "Publish/linux-x64/"
    Write-Output "Ended adding linux-x64 to .zip"
}

# osx-x64
if ($IsMacOS -or $buildAll)
{
    Write-Output "Start adding osx-x64 to .zip"
    Start-Process -NoNewWindow -Wait -FilePath "7z" -ArgumentList "a ../osx-x64.zip *" -WorkingDirectory "Publish/osx-x64/"
    Write-Output "Ended adding osx-x64 to .zip"
}

Write-Output "Ended adding published files to .zip archives"