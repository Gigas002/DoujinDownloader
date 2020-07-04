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
        /// <param name="circle">Artist's circle. Can be null or empty.</param>
        /// <param name="subsection">Subsection. Can be null or empty.</param>
        /// <param name="language">Language. Can be null or empty.</param>
        /// <param name="name">Doujin's name.</param>
        /// <param name="uri">Doujin uri.</param>
        /// <param name="isDownloaded">Is already downloaded?</param>
        //[JsonConstructor]
        //public Doujin(string artist, string circle, string subsection, string language, string name, Uri uri, bool isDownloaded)
        //{
        //    Artist = artist;
        //    Circle = string.IsNullOrWhiteSpace(circle) ? null : circle;
        //    Subsection = string.IsNullOrWhiteSpace(subsection) ? null : subsection;
        //    Language = string.IsNullOrWhiteSpace(language) ? null : language;
        //    Name = name;
        //    Uri = uri;
        //    IsDownloaded = isDownloaded;
        //}

        /// <summary>
        /// Artist name or nickname.
        /// </summary>
        [JsonPropertyName(nameof(Artist))]
        public string Artist { get; set; }

        /// <summary>
        /// Artist circle.
        /// </summary>
        [JsonPropertyName(nameof(Circle))]
        public string Circle { get; set; }

        /// <summary>
        /// Subsection.
        /// </summary>
        [JsonPropertyName(nameof(Subsection))]
        public string Subsection { get; set; }

        /// <summary>
        /// Language.
        /// </summary>
        [JsonPropertyName(nameof(Language))]
        public string Language { get; set; }

        /// <summary>
        /// Doujin name.
        /// </summary>
        [JsonPropertyName(nameof(Name))]
        public string Name { get; set; }

        /// <summary>
        /// Doujin uri.
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
