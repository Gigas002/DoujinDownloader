# DoujinDownloader

Simple app on **.NET Core 3.1** to create and support your local doujins repo. You can build project in **VS2019** (**16.6.2**+) or in **VSCode** (**1.46.1**+) with [omnisharp-vscode](https://github.com/OmniSharp/omnisharp-vscode) extension (**1.22.1+**).

Now only creates file with `uri` list, that you can use with, for example, [HitomiDownloader](https://github.com/KurtBestor/Hitomi-Downloader-issues).

![Icon](DoujinDownloader/Resources/icon.png)

Icon is made by [Freepik](https://www.flaticon.com/authors/freepik) from [FlatIcon](https://www.flaticon.com/).

[![Build status](https://ci.appveyor.com/api/projects/status/8c8nxdm8sniqkxeq?svg=true)](https://ci.appveyor.com/project/Gigas002/doujindownloader) [![Actions Status](https://github.com/Gigas002/DoujinDownloader/workflows/.NET%20Core/badge.svg)](https://github.com/Gigas002/DoujinDownloader/actions)

##  Current version

Current stable can be found here: [![Release](https://img.shields.io/github/release/Gigas002/DoujinDownloader.svg)](https://github.com/Gigas002/DoujinDownloader/releases/latest).

Information about changes since previous releases can be found in [changelog](https://github.com/Gigas002/DoujinDownloader/blob/master/CHANGELOG.md). This project supports [SemVer 2.0.0](https://semver.org/) (template is `{MAJOR}.{MINOR}.{PATCH}.{BUILD}`).

Previous versions can be found on [releases](https://github.com/Gigas002/DoujinDownloader/releases) and [branches](https://github.com/Gigas002/DoujinDownloader/branches) pages.

## Dependencies

- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser/) – 2.8.0;

## Input parameters

| Short name | Long name |        Description         | Required? |
| :--------: | :-------: | :------------------------: | :-------: |
|     -i     |   --input   | Input file (`.md`/`.json`) | Yes |
|     -j     |   --json   | Path to converted from `.md` ``.json` file | No |
| -a | --artist | Artist name to get from input file | No |
| -u | --uris | Path to ready `uris.txt` file | No |
|  | --version | Current version |  |
|  | --help | Message about command line options |  |

`-i/--input` is a `string` with path to `.md` or `.json` file with doujins list.

`-j/--json` is a `string` with path to converted `.json` file with doujins list. If not set – created near `.exe`.

`-a/--artist` is a **Artist’s name** `string` to get from input file. If not set – gets all artist.

`-u/--uris` is a `string` with path to readu `.txt` file with doujins uris. If not set – created near `.exe`.

Simple example looks like this: `DoujinDownloader -i Doujins.md -j Doujins.json -a ArtistName -u Uris.txt`.

## How to use md

```text
## [Author name](uri)
### Subsection //tankoubon, etc -- optional
- [ ] [Doujin name](uri), Language //Not Downloaded
- [x] [Doujin name]() //Downloaded
```

Uri, language and subsection are optional and can be null.

[Example of `.md` file](https://github.com/Gigas002/DoujinDownloader/blob/master/Doujins.md)

## How to use json

[Example of `.json` file](https://github.com/Gigas002/DoujinDownloader/blob/master/Doujins.json)

## Localization

Localizable strings are located in `Localization/Strings.resx` file. You can add your translation (e.g. added `Strings.Ru.resx` file) and create pull request.

Currently, application is available on **English** and **Russian** languages.

## Contributing

Feel free to contribute, make forks, change some code, add [issues](https://github.com/Gigas002/DoujinDownloader/issues), etc.
