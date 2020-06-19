using System.Collections.Generic;
using System.Text.Json.Serialization;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global

namespace DoujinDownloader.HitomiDownloader
{
    /// <summary>
    /// Hitomi.la's gallery info, parsed from json response
    /// </summary>
    public class GalleryInfo
    {
        [JsonPropertyName("language_localname")]
        public string LanguageLocalname { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("files")]
        public List<ImageInfo> Images { get; set; } = new List<ImageInfo>();

        [JsonPropertyName("tags")]
        public List<TagInfo> Tags { get; set; } = new List<TagInfo>();

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
