# DoujinDownloader

Simple app on **.NET 6.0** to create and support your local doujins repo. Auto downloading through [gallery-dl](https://github.com/mikf/gallery-dl) is planned for 2.1 release.

Now only creates file with `url` list, that you can use with, for example, [gallery-dl](https://github.com/mikf/gallery-dl) or [HitomiDownloader](https://github.com/KurtBestor/Hitomi-Downloader-issues).

![Icon](DoujinDownloader/Resources/icon.png)

Icon is made by [Freepik](https://www.flaticon.com/authors/freepik) from [FlatIcon](https://www.flaticon.com/).

[![Build status](https://ci.appveyor.com/api/projects/status/8c8nxdm8sniqkxeq?svg=true)](https://ci.appveyor.com/project/Gigas002/doujindownloader) [![Actions Status](https://github.com/Gigas002/DoujinDownloader/workflows/.NET%20Core/badge.svg)](https://github.com/Gigas002/DoujinDownloader/actions)

## Current version

Current stable can be found here: [![Release](https://img.shields.io/github/release/Gigas002/DoujinDownloader.svg)](https://github.com/Gigas002/DoujinDownloader/releases/latest).

Pre-release versions by CI/CD are also thrown down on [Releases](https://github.com/Gigas002/DoujinDownloader/releases) page.

Information about changes since previous releases can be found in [changelog](https://github.com/Gigas002/DoujinDownloader/blob/master/CHANGELOG.md). This project supports [SemVer 2.0.0](https://semver.org/) (template is `{MAJOR}.{MINOR}.{PATCH}.{BUILD}`).

Previous versions can be found on [releases](https://github.com/Gigas002/DoujinDownloader/releases) and [branches](https://github.com/Gigas002/DoujinDownloader/branches) pages.

## Build

Projects targets **.NET 6.0.0**, so you’ll need **.NET 6.0.100 SDK**.

**Release** binaries are made by `publish-github-release.ps1` script. Take a look at it in the repo. Note, that running this script requires installed **PowerShell** or **[PowerShell Core](https://github.com/PowerShell/PowerShell)** for **Linux**/**OSX** systems.

## Docker Image

Latest pre-built docker images (*from master branch*) for **DoujinDownloader** are available on [GitHub packages](https://github.com/Gigas002/DoujinDownloader/packages/277538) (`docker pull docker.pkg.github.com/gigas002/doujindownloader/doujindownloader:latest`) and on [Docker Hub](https://hub.docker.com/r/gigas002/doujindownloader) (`docker pull gigas002/doujindownloader`).

You can also build docker image by yourself by running `publish-local-docker.ps1` script with your **PowerShell**/**PowerShell Core (on Linux)**. It’ll create `doujindownloader` image.

## Dependencies

- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser/) – 2.8.0;

## Input parameters

| Short name | Long name |                Description                | Required? |
| :--------: | :-------: | :---------------------------------------: | :-------: |
|     -i     |  --input  |        Input file (`.md`/`.json`)         |    No     |
|     -j     |  --json   | Path to converted from `.md` `.json` file |    No     |
|     -u     |  --urls   |       Path to ready `urls.txt` file       |    No     |
|            | --version |              Current version              |           |
|            |  --help   |    Message about command line options     |           |

`-i/--input` is a `string` with path to `.md` or `.json` file with doujins list. If not set -- tries to take `doujins.md` in your running directory.

`-j/--json` is a `string` with path to converted `.json` file with doujins list. If not set -– created near `.exe`.

`-u/--urls` is a `string` with path to ready `.txt` file with doujins urls. If not set -– created near `.exe`.

Simple example looks like this: `DoujinDownloader -i doujins.md -j doujins.json -u uris.txt`.

## How to use md

```text
## [Artist Name 1, Alternative Name 'Artist Circle, Alternative Circle'](https://uri-sample.test); tag1, tag2, tag3 // Circle, url, tags optional
### Subsection // tankoubon, etc -- optional
- [ ] [Doujin name](uri), Language; tag1, tag2 //Not Downloaded
- [x] [Doujin name]() //Downloaded
```

Uri, language and subsection are optional and can be null.

[Example of `.md` file](Doujins.md)

## How to use json

[Example of `.json` file](Doujins.json)

## Localization

Localizable strings are located in `Localization/Strings.resx` file. You can add your translation (e.g. added `Strings.Ru.resx` file) and create pull request.

Currently, application is available on **English** and **Russian** languages.

## Contributing

Feel free to contribute, make forks, change some code, add [issues](https://github.com/Gigas002/DoujinDownloader/issues), etc.
