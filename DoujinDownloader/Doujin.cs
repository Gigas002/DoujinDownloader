using System;
using Newtonsoft.Json;

namespace DoujinDownloader
{
    /// <summary>
    /// Represents doujin object.
    /// </summary>
    internal sealed class Doujin
    {
        /// <summary>
        /// Constructor to create doujin object.
        /// </summary>
        /// <param name="artist">Artist name or nickname.</param>
        /// <param name="subsection">Subsection. Can be null or empty.</param>
        /// <param name="language">Language. Can be null or empty.</param>
        /// <param name="name">Doujin's name.</param>
        /// <param name="uri">Doujin uri.</param>
        /// <param name="isDownloaded">Is already downloaded?</param>
        internal Doujin(string artist, string subsection, string language, string name, Uri uri, bool isDownloaded)
        {
            Artist = artist;
            Subsection = string.IsNullOrWhiteSpace(subsection) ? null : subsection;
            Language = string.IsNullOrWhiteSpace(language) ? null : language;
            Name = name;
            Uri = uri;
            IsDownloaded = isDownloaded;
        }

        /// <summary>
        /// Constructor to create empty doujin object.
        /// </summary>
        internal Doujin() { }

        /// <summary>
        /// Artist name or nickname.
        /// </summary>
        [JsonProperty(nameof(Artist))]
        internal string Artist { get; set; } = string.Empty;

        /// <summary>
        /// Subsection.
        /// </summary>
        [JsonProperty(nameof(Subsection), NullValueHandling = NullValueHandling.Ignore)]
        internal string Subsection { get; set; } = string.Empty;

        /// <summary>
        /// Language.
        /// </summary>
        [JsonProperty(nameof(Language), NullValueHandling = NullValueHandling.Ignore)]
        internal string Language { get; set; } = string.Empty;

        /// <summary>
        /// Doujin name.
        /// </summary>
        [JsonProperty(nameof(Name))]
        internal string Name { get; set; } = string.Empty;

        /// <summary>
        /// Doujin uri.
        /// </summary>
        [JsonProperty(nameof(Uri), NullValueHandling = NullValueHandling.Ignore)]
        internal Uri Uri { get; set; }

        /// <summary>
        /// Is doujin already downloaded?
        /// </summary>
        [JsonProperty(nameof(IsDownloaded))]
        internal bool IsDownloaded { get; set; } = true;
    }
}
