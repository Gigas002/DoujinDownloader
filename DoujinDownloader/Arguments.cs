using CommandLine;

// ReSharper disable All

namespace DoujinDownloader;

/// <summary>
/// Class for parsing console arguments.
/// </summary>
public class Arguments
{
    #region Optional

    /// <summary>
    /// Full path to input file.
    /// </summary>
    [Option('i', "input", Required = false, HelpText = "Full path to input file")]
    public string InputPath { get; set; } = "doujins.md";

    /// <summary>
    /// Path to converted json.
    /// </summary>
    [Option('j', "json", Required = false, HelpText = "Path to converted json")]
    public string JsonPath { get; set; } = "doujins.json";

    /// <summary>
    /// Path to .txt file with raw uris, to use in HitomiDownloader.
    /// </summary>
    [Option('u', "urls", Required = false, HelpText = "Path to .txt file with raw uris")]
    public string UrisPath { get; set; } = "urls.txt";

    #endregion
}