using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Newtonsoft.Json;
using DoujinDownloader.Enums;

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
        /// Artist name.
        /// </summary>
        private static string ArtistName { get; set; }

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

            //Get command line args.
            Parser.Default.ParseArguments<Options>(args).WithParsed(ParseConsoleOptions)
                  .WithNotParsed(error => IsParsingErrors = true);

            //Stop executing if errors occured.
            if (IsParsingErrors) return;

            stopwatch.Start();

            Doujins doujins = new Doujins();

            //If input file extensions .json => deserialize it to Doujins object.
            if (InputFileInfo.Extension == Extensions.JsonExtension)
            {
                try
                {
                    doujins = JsonConvert.DeserializeObject<Doujins>(await File.ReadAllTextAsync(InputFileInfo.FullName)
                                                                               .ConfigureAwait(false));
                }
                catch (Exception)
                {
                    Console.WriteLine($"Unable to deserialize selected {Extensions.JsonExtension} file.");

                    return;
                }

                JsonFileInfo = InputFileInfo;
            }
            //Else if this is .md file => parse it to Doujins object and write to .json file.
            else if (InputFileInfo.Extension == Extensions.MarkdownExtension)
            {
                doujins = await MarkdownParser.ParseMarkdownAsync(InputFileInfo.FullName, ArtistName).ConfigureAwait(false);

                //If no doujins in .md file.
                if (!doujins.DoujinsList.Any())
                {
                    Console.WriteLine($"No doujins selected in {Extensions.MarkdownExtension} file.");

                    return;
                }

                //Write to .json
                await Task.Run(() => WriteJson(doujins)).ConfigureAwait(false);
            }

            //Print some additional info.
            await Task.Run(() => PrintCount(doujins)).ConfigureAwait(false);

            //Test files names => try to write on disk.
            await Task.Run(() => TestDoujinsNames(doujins, Paths.DoujinsDirectoryPath)).ConfigureAwait(false);

            //Write uris to use in HitomiDownloader (for example).
            await Task.Run(() => WriteUrisAsync(doujins, $"{UrisFileInfo.FullName}")).ConfigureAwait(false);

            //TODO
            //Download doujin.

            //After work is done.
            stopwatch.Stop();
            Console.WriteLine($"Done in hours:{stopwatch.Elapsed.Hours}, " +
                              $"minutes:{stopwatch.Elapsed.Minutes}, seconds:{stopwatch.Elapsed.Seconds}, " +
                              $"milliseconds:{stopwatch.Elapsed.Milliseconds}.");
        }

        #region Methods

        /// <summary>
        /// Set properties values from command line options.
        /// </summary>
        /// <param name="options">Command line options.</param>
        private static void ParseConsoleOptions(Options options)
        {
            //Check if string options are empty strings.
            if (string.IsNullOrWhiteSpace(options.InputFilePath))
            {
                Console.WriteLine("Option -i/--input is not set.");
                IsParsingErrors = true;

                return;
            }

            //Set properties values.
            InputFileInfo = new FileInfo(options.InputFilePath);
            ArtistName = options.ArtistName;
            JsonFileInfo = new FileInfo(options.JsonPath);
            UrisFileInfo = new FileInfo(options.UrisPath);

            if (JsonFileInfo.Extension != Extensions.JsonExtension)
            {
                Console.WriteLine($"-j/--json file extensions is not {Extensions.JsonExtension}");
                IsParsingErrors = true;

                return;
            }

            if (UrisFileInfo.Extension != Extensions.TxtExtension)
            {
                Console.WriteLine($"-u/--uris file extensions is not {Extensions.TxtExtension}");
                IsParsingErrors = true;

                return;
            }

            if (InputFileInfo.Extension == Extensions.JsonExtension ||
                InputFileInfo.Extension == Extensions.MarkdownExtension)
                return;

            Console.WriteLine($"-i/--input file extensions is not {Extensions.JsonExtension}/{Extensions.MarkdownExtension}");
            IsParsingErrors = true;
        }

        /// <summary>
        /// Writes .json file from <see cref="Doujins"/> object.
        /// </summary>
        /// <param name="doujins">Object with <see cref="Doujin"/> list.</param>
        private static void WriteJson(Doujins doujins)
        {
            JsonFileInfo.Directory?.Create();

            using (StreamWriter streamWriter = JsonFileInfo.CreateText())
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(streamWriter, doujins);
            }

            JsonFileInfo.Refresh();
        }

        /// <summary>
        /// Prints count of doujins list.
        /// </summary>
        /// <param name="doujins">Object with <see cref="Doujin"/> list.</param>
        private static void PrintCount(Doujins doujins)
        {
            Console.WriteLine($"Doujins count:{doujins.DoujinsList.Count}");
            Console.WriteLine($"Doujins to download count:{GetDoujinsToDownload(doujins).Count()}");
        }

        /// <summary>
        /// Test method for writing test .txt files from doujins list on disk.
        /// </summary>
        /// <param name="doujins">Object with <see cref="Doujin"/> list.</param>
        /// <param name="doujinsDirectoryPath">Directory for doujin files.</param>
        private static void TestDoujinsNames(Doujins doujins, string doujinsDirectoryPath)
        {
            #if RELEASE
            return;
            #endif

            foreach (Doujin doujin in doujins.DoujinsList)
            {
                string subsection = string.IsNullOrWhiteSpace(doujin.Subsection) ? string.Empty : doujin.Subsection;
                string language = string.IsNullOrWhiteSpace(doujin.Language) ? string.Empty : doujin.Language;

                string path = Path.Combine(doujinsDirectoryPath, doujin.Artist, subsection, doujin.Name, language,
                                           $"{doujin.Name}{Extensions.TxtExtension}");
                FileInfo doujinFileInfo = new FileInfo(path);

                try
                {
                    doujinFileInfo.Directory?.Create();
                    doujinFileInfo.Create();
                }
                catch (Exception) { Console.WriteLine($"Wrong path:{path}"); }
            }
        }

        /// <summary>
        /// Writes only all uris to download from <see cref="Doujins"/> object to text file.
        /// </summary>
        /// <param name="doujins">Object with <see cref="Doujin"/> list.</param>
        /// <param name="urisTxtPath">Full path to uris.txt file..</param>
        private static async ValueTask WriteUrisAsync(Doujins doujins, string urisTxtPath)
        {
            List<Doujin> doujinsList = GetDoujinsToDownload(doujins).ToList();

            try
            {
                FileInfo txtFileInfo = new FileInfo(urisTxtPath);
                if (txtFileInfo.Exists) txtFileInfo.Delete();
                txtFileInfo.Directory?.Create();
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to create directory:{urisTxtPath} or delete file:{urisTxtPath}.");
            }

            for (int i = 0; i < doujinsList.Count; i++)
            {
                try
                {
                    string uriToAppend = i == doujinsList.Count - 1
                                             ? $"{doujinsList[i].Uri}"
                                             : $"{doujinsList[i].Uri}, ";
                    await File.AppendAllTextAsync(urisTxtPath, uriToAppend).ConfigureAwait(false);
                }
                catch (Exception) { Console.WriteLine($"Unable to append uri {doujinsList[i].Uri} to file."); }
            }
        }

        /// <summary>
        /// Gets list of doujins to download.
        /// </summary>
        /// <param name="doujins">Object with <see cref="Doujin"/> list.</param>
        /// <returns>List of <see cref="Doujin"/> objects.</returns>
        private static IEnumerable<Doujin> GetDoujinsToDownload(Doujins doujins) =>
            //TODO: test AsParallel and IAsyncEnumerable
            doujins.DoujinsList.Where(doujin => doujin.Uri != null && !doujin.IsDownloaded);

        #endregion
    }
}
