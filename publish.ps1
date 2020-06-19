Write-Output "Started building/publishing"

# DoujinDownloader

# Win-x64
Write-Output "Start Win-x64 publish"
dotnet publish "DoujinDownloader/DoujinDownloader.csproj" -c Release -r win-x64 -o Publish/win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
Write-Output "Ended Win-x64 publish"

# Win-x86
Write-Output "Start Win-x86 publish"
dotnet publish "DoujinDownloader/DoujinDownloader.csproj" -c Release -r win-x86 -o Publish/win-x86 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
Write-Output "Ended Win-x86 publish"

# Linux-x64
Write-Output "Start Linux-x64-Benchmarks publish"
dotnet publish "DoujinDownloader/DoujinDownloader.csproj" -c Release -r linux-x64 -o Publish/linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
Write-Output "Ended Linux-x64-Benchmarks publish"

# Osx-x64
Write-Output "Start Osx-x64-Benchmarks publish"
dotnet publish "DoujinDownloader/DoujinDownloader.csproj" -c Release -r osx-x64 -o Publish/osx-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
Write-Output "Ended Osx-x64-Benchmarks publish"

Write-Output "Ended building/publishing"

# Remove all *.pdb files
Write-Output "Removing all *.pdb files from Publish directory"
Get-ChildItem "Publish/" -Include *.pdb -Recurse | Remove-Item

# Copy docs, etc to published directories before zipping them
Write-Output "Copying docs, license, etc to published directories"

Copy-Item -Path "DoujinDownloader/Readme.pdf" -Destination "Publish/win-x64/Readme.pdf"
Copy-Item -Path "LICENSE" -Destination "Publish/win-x64/LICENSE"
Copy-Item -Path "CHANGELOG.md" -Destination "Publish/win-x64/CHANGELOG.md"

Copy-Item -Path "DoujinDownloader/Readme.pdf" -Destination "Publish/win-x86/Readme.pdf"
Copy-Item -Path "LICENSE" -Destination "Publish/win-x86/LICENSE"
Copy-Item -Path "CHANGELOG.md" -Destination "Publish/win-x86/CHANGELOG.md"

Copy-Item -Path "DoujinDownloader/Readme.pdf" -Destination "Publish/linux-x64/Readme.pdf"
Copy-Item -Path "LICENSE" -Destination "Publish/linux-x64/LICENSE"
Copy-Item -Path "CHANGELOG.md" -Destination "Publish/linux-x64/CHANGELOG.md"

Copy-Item -Path "DoujinDownloader/Readme.pdf" -Destination "Publish/osx-x64/Readme.pdf"
Copy-Item -Path "LICENSE" -Destination "Publish/osx-x64/LICENSE"
Copy-Item -Path "CHANGELOG.md" -Destination "Publish/osx-x64/CHANGELOG.md"

# Add everything into archives. Requires installed 7z added to PATH.

$7zStartInfo = New-Object System.Diagnostics.ProcessStartInfo
$7zStartInfo.FileName = "7z"
$7zStartInfo.CreateNoWindow = $true
$process = New-Object System.Diagnostics.Process
$process.StartInfo = $7zStartInfo

Write-Output "Started adding published files to .zip archives"

# Add win-x64 to .zip
Write-Output "Start adding win-x64 to .zip"
$7zStartInfo.Arguments = "a ../win-x64.zip *"
$7zStartInfo.WorkingDirectory = "Publish/win-x64/"
$process.Start() # | Out-Null
$process.WaitForExit()
Write-Output "Ended adding win-x64 to .zip"

# Add win-x86 to .zip
Write-Output "Start adding win-x86 to .zip"
$7zStartInfo.Arguments = "a ../win-x86.zip *"
$7zStartInfo.WorkingDirectory = "Publish/win-x86/"
$process.Start() # | Out-Null
$process.WaitForExit()
Write-Output "Ended adding win-x86 to .zip"

# Add linux-x64 to .zip
Write-Output "Start adding linux-x64 to .zip"
$7zStartInfo.Arguments = "a ../linux-x64.zip *"
$7zStartInfo.WorkingDirectory = "Publish/linux-x64/"
$process.Start() # | Out-Null
$process.WaitForExit()
Write-Output "Ended adding linux-x64 to .zip"

# Add osx-x64 to .zip
Write-Output "Start adding osx-x64 to .zip"
$7zStartInfo.Arguments = "a ../osx-x64.zip *"
$7zStartInfo.WorkingDirectory = "Publish/osx-x64/"
$process.Start() # | Out-Null
$process.WaitForExit()
Write-Output "Ended adding osx-x64 to .zip"

Write-Output "Ended adding published files to .zip archives"
