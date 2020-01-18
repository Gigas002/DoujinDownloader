using System;
using System.Text.Json.Serialization;

namespace DoujinDownloader
{
    /// <summary>
    /// Represents doujin object.
    /// </summary>
    public sealed class Doujin
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
        /// Artist name or nickname.
        /// </summary>
        [JsonPropertyName(nameof(Artist))]
        public string Artist { get; }

        /// <summary>
        /// Subsection.
        /// </summary>
        [JsonPropertyName(nameof(Subsection))]
        public string Subsection { get; }

        /// <summary>
        /// Language.
        /// </summary>
        [JsonPropertyName(nameof(Language))]
        public string Language { get; }

        /// <summary>
        /// Doujin name.
        /// </summary>
        [JsonPropertyName(nameof(Name))]
        public string Name { get; }

        /// <summary>
        /// Doujin uri.
        /// </summary>
        [JsonPropertyName(nameof(Uri))]
        public Uri Uri { get; }

        /// <summary>
        /// Is doujin already downloaded?
        /// </summary>
        [JsonPropertyName(nameof(IsDownloaded))]
        public bool IsDownloaded { get; }
    }
}
