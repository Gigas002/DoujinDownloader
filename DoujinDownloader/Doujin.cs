using System;
using System.Text.Json.Serialization;

namespace DoujinDownloader
{
    /// <summary>
    /// Represents doujin object
    /// </summary>
    public sealed class Doujin
    {
        /// <summary>
        /// Artist name or nickname
        /// </summary>
        [JsonPropertyName(nameof(Artist))]
        public string Artist { get; set; }

        /// <summary>
        /// Artist circle
        /// </summary>
        [JsonPropertyName(nameof(Circle))]
        public string Circle { get; set; }

        /// <summary>
        /// Subsection
        /// </summary>
        [JsonPropertyName(nameof(Subsection))]
        public string Subsection { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        [JsonPropertyName(nameof(Language))]
        public string Language { get; set; }

        /// <summary>
        /// Doujin name
        /// </summary>
        [JsonPropertyName(nameof(Name))]
        public string Name { get; set; }

        /// <summary>
        /// Doujin uri
        /// </summary>
        [JsonPropertyName(nameof(Uri))]
        public Uri Uri { get; set; }

        /// <summary>
        /// Is doujin already downloaded?
        /// </summary>
        [JsonPropertyName(nameof(IsDownloaded))]
        public bool IsDownloaded { get; set; }
    }
}
