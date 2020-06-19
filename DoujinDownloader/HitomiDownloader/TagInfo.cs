using System.Text.Json.Serialization;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace DoujinDownloader.HitomiDownloader
{
    /// <summary>
    /// Tag's info from hitomi.la's response
    /// </summary>
    public class TagInfo
    {
        [JsonPropertyName("female")]
        public string Female { get; set; }

        [JsonPropertyName("male")]
        public string Male { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        public TagInfo(string female, string male, string url, string tag)
        {
            Female = female;
            Male = male;
            Url = url;
            Tag = tag;
        }
    }
}
