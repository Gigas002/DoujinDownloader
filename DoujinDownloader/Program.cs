using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommandLine;
using DoujinDownloader.Constants;
using DoujinDownloader.Localization;

namespace DoujinDownloader;

internal static class Program
{
    #region Properties

    /// <summary>
    /// .md or .json file with doujin list.
    /// </summary>
    private static string InputPath { get; set; }

    /// <summary>
    /// Converted .json (if input was .md).
    /// </summary>
    private static string JsonPath { get; set; }

    /// <summary>
    /// .txt file with raw uris, to use in (for example) HitomiDownloader.
    /// </summary>
    private static string UrlsPath { get; set; }

    /// <summary>
    /// Shows if there were command line parsing errors.
    /// </summary>
    private static bool IsParsingErrors { get; set; }

    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    #endregion

    internal static void Main(string[] args)
    {
        var stopwatch = new Stopwatch();

        //Get command line args
        Parser.Default.ParseArguments<Arguments>(args).WithParsed(ParseConsoleOptions).WithNotParsed(_ => IsParsingErrors = true);

        //Stop executing if errors occured
        if (IsParsingErrors) return;

        stopwatch.Start();

        try
        {
            ParseAndDownloadDoujins();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);

            #if DEBUG
            Console.WriteLine(exception.InnerException?.Message);
            #endif

            return;
        }

        //After work is done
        stopwatch.Stop();
        Console.WriteLine(Strings.Done, stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds, stopwatch.Elapsed.Milliseconds);
    }

    #region Methods

    /// <summary>
    /// Set properties values from command line options
    /// </summary>
    /// <param name="arguments">Command line args</param>
    private static void ParseConsoleOptions(Arguments arguments)
    {
        // Check if string options are empty strings
        if (string.IsNullOrWhiteSpace(arguments.InputPath))
        {
            Console.WriteLine(Strings.InputNotSet);
            IsParsingErrors = true;

            return;
        }

        // Set properties values
        InputPath = arguments.InputPath;
        JsonPath = arguments.JsonPath;
        UrlsPath = arguments.UrisPath;

        if (Path.GetExtension(JsonPath) != Extensions.JsonExtension)
        {
            Console.WriteLine(Strings.JsonWrongExtension, Extensions.JsonExtension);
            IsParsingErrors = true;

            return;
        }

        if (Path.GetExtension(UrlsPath) != Extensions.TxtExtension)
        {
            Console.WriteLine(Strings.UrisWrongExtension, Extensions.TxtExtension);
            IsParsingErrors = true;

            return;
        }

        if (Path.GetExtension(InputPath) == Extensions.JsonExtension ||
            Path.GetExtension(InputPath) == Extensions.MarkdownExtension)
            return;

        Console.WriteLine(Strings.InputWrongExtension, Extensions.JsonExtension, Extensions.MarkdownExtension);
        IsParsingErrors = true;
    }

    private static void ParseAndDownloadDoujins()
    {
        List<Doujin> doujins;

        var extension = Path.GetExtension(InputPath);

        switch (extension)
        {
            case Extensions.JsonExtension:
            {
                using var fs = File.OpenRead(InputPath);
                doujins = JsonSerializer.Deserialize<IEnumerable<Doujin>>(fs, JsonOptions).ToList();

                // If no doujins in .json file
                if (!doujins.Any())
                {
                    Console.WriteLine(Strings.NoDoujinsInInput, Extensions.JsonExtension);

                    return;
                }

                JsonPath = InputPath;

                break;
            }
            case Extensions.MarkdownExtension:
            {
                var mdLines = File.ReadAllLines(InputPath);
                doujins = MarkdownParser.ParseMarkdownAsync(mdLines).ToList();

                // If no doujins in .md file
                if (!doujins.Any())
                {
                    Console.WriteLine(Strings.NoDoujinsInInput, Extensions.MarkdownExtension);

                    return;
                }

                // Write to .json
                using var fs = File.OpenWrite(JsonPath);
                JsonSerializer.Serialize(fs, doujins, JsonOptions);

                break;
            }
            default:
            {
                Console.WriteLine(Strings.NotSupported, extension);

                return;
            }
        }

        // Print some additional info
        PrintCount(doujins);

        // Write urls file
        WriteUrls(doujins, $"{UrlsPath}");

        //TODO: Download doujins through gallery-dl
    }

    private static void PrintCount(IEnumerable<Doujin> doujins)
    {
        var djs = doujins.ToList();

        Console.WriteLine(Strings.DoujinsCount, djs.Count);
        Console.WriteLine(Strings.DoujinsToDownloadCount, GetDoujinsToDownload(djs).Count());
    }

    private static void WriteUrls(IEnumerable<Doujin> doujins, string urlsTxtPath)
    {
        var doujinsList = GetDoujinsToDownload(doujins).ToList();

        if (File.Exists(urlsTxtPath)) File.Delete(urlsTxtPath);
        // txtFileInfo.Directory?.Create();
        using var fs = File.OpenWrite(urlsTxtPath);

        foreach (var doujin in doujinsList)
        {
            var buffer = Encoding.UTF8.GetBytes($"{doujin.Url}\n");
            fs.Write(buffer);
        }
    }

    private static IEnumerable<Doujin> GetDoujinsToDownload(IEnumerable<Doujin> doujins) =>
        doujins.AsParallel().Where(doujin => doujin.Url != null && !doujin.IsDownloaded);

    #endregion
}