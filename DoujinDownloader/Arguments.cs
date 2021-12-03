using CommandLine;

// ReSharper disable All

namespace DoujinDownloader;

/// <summary>
/// Class for parsing console arguments.
/// </summary>
public class Arguments
{
    #region Required

    /// <summary>
    /// Full path to input file.
    /// </summary>
    [Option('i', "input", Required = true, HelpText = "Full path to input file.")]
    public string InputFilePath { get; set; }

    #endregion

    #region Optional

    /// <summary>
    /// Path to converted json.
    /// </summary>
    [Option('j', "json", Required = false, HelpText = "Path to converted json.")]
    public string JsonPath { get; set; } = "Doujins.json";

    /// <summary>
    /// Path to .txt file with raw uris, to use in HitomiDownloader.
    /// </summary>
    [Option('u', "uris", Required = false, HelpText = "Path to .txt file with raw uris, to use in HitomiDownloader.")]
    public string UrisPath { get; set; } = "Uris.txt";

    #endregion
}
