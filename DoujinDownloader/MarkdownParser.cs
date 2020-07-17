using DoujinDownloader.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Local

namespace DoujinDownloader
{
    /// <summary>
    /// Class to parse .md file into <see cref="Doujins"/> object
    /// </summary>
    internal static class MarkdownParser
    {
        #region Constants

        /// <summary>
        /// [name](link), language
        /// </summary>
        private const string DoujinNamePattern = @"^\[(?'name'.*)\]\((?'link'.*)\)\,?\ ?(?'language'.*)";

        /// <summary>
        /// [name](link)
        /// </summary>
        private const string ArtistNamePattern = @"^## \[(?'name'.*)\]\((?'link'.*)\)";

        /// <summary>
        /// Downloaded doujin line starting symbols
        /// </summary>
        private const string CheckBoxChecked = "- [x] ";

        /// <summary>
        /// Not downloaded doujin line starting symbols
        /// </summary>
        private const string CheckBoxUnchecked = "- [ ] ";

        /// <summary>
        /// Document's header line starting symbols
        /// </summary>
        private const string HeaderPattern = "# ";

        /// <summary>
        /// Artist and circle name line starting symbols
        /// </summary>
        private const string ArtistPattern = "## ";

        /// <summary>
        /// Subsection line starting symbols
        /// </summary>
        private const string SubsectionPattern = "### ";

        #endregion

        #region Methods

        /// <summary>
        /// Parse <see cref="Doujins"/> .md file into <see cref="Doujins"/> list
        /// </summary>
        /// <param name="markdownFilePath">Full path to .md file</param>
        /// <returns><see cref="Doujins"/> object with <see cref="Doujin"/> collection</returns>
        internal static async ValueTask<Doujins> ParseMarkdownAsync(string markdownFilePath)
        {
            IEnumerable<string> markdownLines = await File.ReadAllLinesAsync(markdownFilePath, Encoding.UTF8).ConfigureAwait(false);

            return await ParseMarkdownAsync(markdownLines).ConfigureAwait(false);
        }

        /// <summary>
        /// Parse lines from <see cref="Doujins"/> .md file into <see cref="Doujins"/> list
        /// </summary>
        /// <param name="markdownLines">Lines from <see cref="Doujins"/> .md file</param>
        /// <returns><see cref="Doujins"/> object with <see cref="Doujin"/> collection</returns>
        private static async ValueTask<Doujins> ParseMarkdownAsync(IEnumerable<string> markdownLines)
        {
            Doujins doujins = new Doujins();

            string artist = null;
            string circle = null;
            string subsection = null;

            const string subsectionShortStart = "###";
            const string doujinShortStart = "- [";

            // Don't pause the main thread's work
            await Task.Run(() =>
            {
                foreach (string markdownLine in markdownLines.Where(markdownLine =>
                                                                        !string.IsNullOrWhiteSpace(markdownLine)))
                {
                    string lineStart = markdownLine.Substring(0, 3);

                    switch (lineStart)
                    {
                        case ArtistPattern:
                        {
                            (artist, circle) = ParseArtistCircleLine(markdownLine);

                            //if new artist = mark subsection empty
                            subsection = null;

                            break;
                        }
                        case subsectionShortStart:
                        {
                            subsection = markdownLine.Remove(0, 4).Trim();

                            break;
                        }
                        case doujinShortStart:
                        {
                            doujins.DoujinsList.Add(CreateDoujin(markdownLine, artist, circle, subsection));

                            break;
                        }
                    }
                }
            }).ConfigureAwait(false);

            return doujins;
        }

        /// <summary>
        /// Creates one doujin from markdown line
        /// </summary>
        /// <param name="markdownLine">Line to parse</param>
        /// <param name="artist">Artist name</param>
        /// <param name="circle">Circle name
        /// <para/>Optional, <see langword="null"/> by default</param>
        /// <param name="subsection">Tankoubon, etc
        /// <para/>Optional, <see langword="null"/> by default</param>
        /// <returns>New <see cref="Doujin"/> object</returns>
        private static Doujin CreateDoujin(string markdownLine, string artist, string circle = null,
                                           string subsection = null)
        {
            //Get doujin name and stuff
            string name;
            Uri uri;
            string language;
            bool isDownloaded;

            (name, uri, language, isDownloaded) = ParseDoujinLine(markdownLine.Trim());

            return new Doujin
            {
                Artist = artist, Circle = circle, Subsection = subsection,
                Language = language, Name = name, Uri = uri,
                IsDownloaded = isDownloaded
            };
        }

        /// <summary>
        /// Parse artist and circle from markdown line
        /// </summary>
        /// <param name="markdownLine">Artist line in .md file</param>
        /// <returns><see cref="ValueTuple{T1, T2}"/> with artist and circle</returns>
        private static (string artist, string circle) ParseArtistCircleLine(
            string markdownLine)
        {
            Match match = Regex.Match(markdownLine, ArtistNamePattern);
            if (!match.Success) throw new Exception(string.Format(Strings.CantParse, nameof(markdownLine), markdownLine));

            string artistCircleLine = match.Groups["name"].Value;
            string artist = artistCircleLine;
            int circleStart = artistCircleLine.IndexOf('\'');

            if (circleStart == -1) return (artist, null);

            string circle = artistCircleLine.Remove(0, circleStart).Replace("\'", string.Empty);
            artist = artistCircleLine.Replace(circle, string.Empty).Replace("\'", string.Empty).Trim();

            return (artist, circle);
        }

        /// <summary>
        /// Parse markdown line from .md file
        /// </summary>
        /// <param name="markdownLine">Doujin line in .md file</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> with doujin name, uri,
        /// language and isDownload</returns>
        private static (string name, Uri uri, string language, bool isDownloaded) ParseDoujinLine(string markdownLine)
        {
            bool isDownloaded = IsDownloadedDoujin(markdownLine);

            //Remove "- [ ] "/"- [x] "
            string name = markdownLine.Remove(0, 6).Trim();

            Match match = Regex.Match(name, DoujinNamePattern);

            if (!match.Success) throw new Exception(string.Format(Strings.CantParse, nameof(markdownLine), markdownLine));

            name = match.Groups[nameof(name)].Value;
            string link = match.Groups[nameof(link)].Value;
            string language = match.Groups[nameof(language)].Value;
            language = string.IsNullOrWhiteSpace(language) ? null : language;
            Uri uri = string.IsNullOrWhiteSpace(link) ? null : new Uri(link);

            return (name, uri, language, isDownloaded);
        }

        /// <summary>
        /// Check if doujin is already downloaded
        /// </summary>
        /// <param name="markdownLine">Doujin line in .md file</param>
        /// <returns><see langword="true"/> if doujin already downloaded;
        /// <see langword="false"/> otherwise</returns>
        private static bool IsDownloadedDoujin(string markdownLine) =>
            markdownLine.StartsWith(CheckBoxChecked, StringComparison.Ordinal);

        #endregion
    }
}
