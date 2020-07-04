using DoujinDownloader.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DoujinDownloader
{
    /// <summary>
    /// Class to parse .md file with doujins list.
    /// </summary>
    internal static class MarkdownParser
    {
        /// <summary>
        /// Parse doujin .md file with doujins list.
        /// </summary>
        /// <param name="markdownFilePath">Full path to .md file.</param>
        /// <param name="artistToDownload">Optional. Name of artist to download.</param>
        /// <returns><see cref="Doujins"/> object with <see cref="Doujin"/> collection.</returns>
        internal static async ValueTask<Doujins> ParseMarkdownAsync(string markdownFilePath, string artistToDownload = "")
        {
            IEnumerable<string> markdownLines =
                await File.ReadAllLinesAsync(markdownFilePath, Encoding.UTF8).ConfigureAwait(false);
            string artist = string.Empty;
            string circle = string.Empty;
            string subsection = string.Empty;

            Doujins doujins = new Doujins();

            foreach (string markdownLine in markdownLines
                                           .Where(markdownLine => !string.IsNullOrWhiteSpace(markdownLine))
                                           .Where(markdownLine =>
                                                      !markdownLine.StartsWith(Constants.MarkdownParser.HeaderPattern,
                                                                               StringComparison.Ordinal)))
            {
                //if artist name line
                if (markdownLine.StartsWith(Constants.MarkdownParser.ArtistPattern, StringComparison.Ordinal))
                {
                    artist = await ParseArtistLineAsync(markdownLine).ConfigureAwait(false);
                    circle = string.Empty;

                    int circleStart = artist.IndexOf('\'');

                    if (circleStart != -1)
                    {
                        circle = artist.Remove(0, circleStart).Replace("\'", string.Empty);
                        artist = artist.Replace(circle, string.Empty).Replace("\'", string.Empty).Trim();
                    }

                    //if new artist = mark subsection empty
                    subsection = string.Empty;

                    continue;
                }

                //if not selected artist
                if (!string.IsNullOrWhiteSpace(artistToDownload))
                    if (!artist.ToLowerInvariant().Contains(artistToDownload.ToLowerInvariant()))
                        continue;

                //if subsection line
                if (markdownLine.StartsWith(Constants.MarkdownParser.SubsectionPattern, StringComparison.Ordinal))
                {
                    subsection = markdownLine.Remove(0, 4).Trim();

                    continue;
                }

                //Skip additional lines just in case
                if (!markdownLine.StartsWith(Constants.MarkdownParser.CheckBoxUnchecked, StringComparison.Ordinal) &&
                    !markdownLine.StartsWith(Constants.MarkdownParser.CheckBoxChecked, StringComparison.Ordinal))
                    continue;

                //Get doujin name and stuff
                string name;
                Uri uri;
                string language;
                bool isDownloaded;

                (name, uri, language, isDownloaded) = await Task.Run(() => ParseDoujinLine(markdownLine.Trim())).ConfigureAwait(false);

                Doujin doujin = new Doujin
                {
                    Artist = artist, Circle = string.IsNullOrWhiteSpace(circle) ? null : circle,
                    Subsection = string.IsNullOrWhiteSpace(subsection) ? null : subsection,
                    Language = string.IsNullOrWhiteSpace(language) ? null : language, Name = name, Uri = uri,
                    IsDownloaded = isDownloaded
                };

                doujins.DoujinsList.Add(doujin);
            }

            return doujins;
        }

        /// <summary>
        /// Parse artist line from .md file.
        /// </summary>
        /// <param name="markdownLine">Artist line in .md file.</param>
        /// <returns>Artist name string.</returns>
        private static async ValueTask<string> ParseArtistLineAsync(string markdownLine) => await Task.Run(() =>
        {
            Match match = Regex.Match(markdownLine, Constants.MarkdownParser.ArtistNamePattern);

            if (match.Success) return match.Groups["name"].Value;

            throw new Exception(string.Format(Strings.CantParse, nameof(markdownLine), markdownLine));
        }).ConfigureAwait(false);

        /// <summary>
        /// Parse doujin line from .md file.
        /// </summary>
        /// <param name="markdownLine">Doujin line in .md file.</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> with doujin name, uri, language and isDownload.</returns>
        private static (string name, Uri uri, string language, bool isDownloaded) ParseDoujinLine(string markdownLine)
        {
            bool isDownloaded = IsDownloadedDoujin(markdownLine);

            //Remove "- [ ] "/"- [x] "
            string name = markdownLine.Remove(0, 6).Trim();

            Match match = Regex.Match(name, Constants.MarkdownParser.DoujinNamePattern);

            if (!match.Success) throw new Exception(string.Format(Strings.CantParse, nameof(markdownLine), markdownLine));

            name = match.Groups[nameof(name)].Value;
            string link = match.Groups[nameof(link)].Value;
            string language = match.Groups[nameof(language)].Value;
            Uri uri = string.IsNullOrWhiteSpace(link) ? null : new Uri(link);

            return (name, uri, language, isDownloaded);
        }

        /// <summary>
        /// Check if doujin is already downloaded.
        /// </summary>
        /// <param name="markdownLine">Doujin line in .md file.</param>
        /// <returns><see langword="true"/> if doujin already downloaded, <see langword="false"/> otherwise.</returns>
        private static bool IsDownloadedDoujin(string markdownLine) =>
            markdownLine.StartsWith(Constants.MarkdownParser.CheckBoxChecked, StringComparison.Ordinal);
    }
}
