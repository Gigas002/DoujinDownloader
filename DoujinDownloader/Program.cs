using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Text.Json;
using DoujinDownloader.Constants;
using DoujinDownloader.Localization;

namespace DoujinDownloader
{
    internal static class Program
    {
        #region Properties

        /// <summary>
        /// .md or .json file with doujin list.
        /// </summary>
        private static FileInfo InputFileInfo { get; set; }

        /// <summary>
        /// Converted .json (if input was .md).
        /// </summary>
        private static FileInfo JsonFileInfo { get; set; }

        /// <summary>
        /// .txt file with raw uris, to use in (for example) HitomiDownloader.
        /// </summary>
        private static FileInfo UrisFileInfo { get; set; }

        /// <summary>
        /// Shows if there were command line parsing errors.
        /// </summary>
        private static bool IsParsingErrors { get; set; }

        #endregion

        internal static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            //Get command line args
            Parser.Default.ParseArguments<Arguments>(args).WithParsed(ParseConsoleOptions)
                  .WithNotParsed(error => IsParsingErrors = true);

            //Stop executing if errors occured
            if (IsParsingErrors) return;

            stopwatch.Start();

            try { await StartWork().ConfigureAwait(false); }
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
        /// <param name="options">Command line options</param>
        private static void ParseConsoleOptions(Arguments options)
        {
            //Check if string options are empty strings.
            if (string.IsNullOrWhiteSpace(options.InputFilePath))
            {
                Console.WriteLine(Strings.InputNotSet);
                IsParsingErrors = true;

                return;
            }

            //Set properties values.
            InputFileInfo = new FileInfo(options.InputFilePath);
            JsonFileInfo = new FileInfo(options.JsonPath);
            UrisFileInfo = new FileInfo(options.UrisPath);

            if (JsonFileInfo.Extension != Extensions.JsonExtension)
            {
                Console.WriteLine(Strings.JsonWrongExtension, Extensions.JsonExtension);
                IsParsingErrors = true;

                return;
            }

            if (UrisFileInfo.Extension != Extensions.TxtExtension)
            {
                Console.WriteLine(Strings.UrisWrongExtension, Extensions.TxtExtension);
                IsParsingErrors = true;

                return;
            }

            if (InputFileInfo.Extension == Extensions.JsonExtension ||
                InputFileInfo.Extension == Extensions.MarkdownExtension)
                return;

            Console.WriteLine(Strings.InputWrongExtension, Extensions.JsonExtension, Extensions.MarkdownExtension);
            IsParsingErrors = true;
        }

        /// <summary>
        /// Makes all the work async
        /// </summary>
        /// <returns></returns>
        private static async ValueTask StartWork()
        {
            Doujins doujins;

            switch (InputFileInfo.Extension)
            {
                case Extensions.JsonExtension:
                {
                    doujins = await JsonSerializer.DeserializeAsync<Doujins>(InputFileInfo.OpenRead())
                                                  .ConfigureAwait(false) ?? new Doujins();

                    //If no doujins in .json file.
                    if (!doujins.DoujinsList.Any())
                    {
                        Console.WriteLine(Strings.NoDoujinsInInput, Extensions.JsonExtension);

                        return;
                    }

                    JsonFileInfo = InputFileInfo;

                    break;
                }
                case Extensions.MarkdownExtension:
                {
                    doujins = await MarkdownParser.ParseMarkdownAsync(InputFileInfo.FullName).ConfigureAwait(false);

                    //If no doujins in .md file.
                    if (!doujins.DoujinsList.Any())
                    {
                        Console.WriteLine(Strings.NoDoujinsInInput, Extensions.MarkdownExtension);

                        return;
                    }

                    //Write to .json
                    await WriteJsonAsync(doujins).ConfigureAwait(false);

                    break;
                }
                default:
                {
                    Console.WriteLine(Strings.NotSupported, InputFileInfo.Extension);

                    return;
                }
            }

            //Print some additional info
            await PrintCountAsync(doujins).ConfigureAwait(false);

            //Write uris to use in HitomiDownloader (for example)
            await WriteUrisAsync(doujins, $"{UrisFileInfo.FullName}").ConfigureAwait(false);

            //TODO
            //Download doujins
        }

        /// <summary>
        /// Writes .json file from <see cref="Doujins"/> object
        /// </summary>
        /// <param name="doujins">Object with <see cref="Doujin"/> list</param>
        private static async ValueTask WriteJsonAsync(Doujins doujins) => await Task.Run(async () =>
        {
            JsonFileInfo.Directory?.Create();

            await using FileStream fileStream = JsonFileInfo.OpenWrite();
            await JsonSerializer.SerializeAsync(fileStream, doujins).ConfigureAwait(false);

            JsonFileInfo.Refresh();
        }).ConfigureAwait(false);

        /// <summary>
        /// Prints count of doujins list and doujins to download
        /// </summary>
        /// <param name="doujins">Object with <see cref="Doujin"/> list</param>
        private static async ValueTask PrintCountAsync(Doujins doujins) => await Task.Run(() =>
        {
            Console.WriteLine(Strings.DoujinsCount, doujins.DoujinsList.Count);
            Console.WriteLine(Strings.DoujinsToDownloadCount, GetDoujinsToDownload(doujins).Count());
        }).ConfigureAwait(false);

        /// <summary>
        /// Writes all uris to download from <see cref="Doujins"/> object to text file
        /// </summary>
        /// <param name="doujins">Object with <see cref="Doujin"/> list</param>
        /// <param name="urisTxtPath">Full path to uris.txt file</param>
        private static async ValueTask WriteUrisAsync(Doujins doujins, string urisTxtPath)
        {
            List<Doujin> doujinsList = GetDoujinsToDownload(doujins).ToList();

            FileInfo txtFileInfo = new FileInfo(urisTxtPath);
            if (txtFileInfo.Exists) txtFileInfo.Delete();
            txtFileInfo.Directory?.Create();
            await using FileStream fileStream = txtFileInfo.OpenWrite();

            foreach (Doujin doujin in doujinsList)
            {
                byte[] buffer = Encoding.UTF8.GetBytes($"{doujin.Uri} ");
                await fileStream.WriteAsync(buffer).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets list of <see cref="Doujin"/>s to download
        /// </summary>
        /// <param name="doujins">Object with <see cref="Doujin"/> list</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="Doujin"/> objects</returns>
        private static IEnumerable<Doujin> GetDoujinsToDownload(Doujins doujins) =>
            doujins.DoujinsList.AsParallel().Where(doujin => doujin.Uri != null && !doujin.IsDownloaded);

        #endregion
    }
}
