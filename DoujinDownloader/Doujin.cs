using System.Text.Json.Serialization;

namespace DoujinDownloader;

public class Doujin
{
    [JsonPropertyName(nameof(Author))]
    public DoujinAuthor Author { get; set; }

    [JsonPropertyName(nameof(Subsection))]
    public string Subsection { get; set; }

    [JsonPropertyName(nameof(Language))]
    public string Language { get; set; }

    [JsonPropertyName(nameof(Name))]
    public string Name { get; set; }

    [JsonPropertyName(nameof(Url))]
    public Uri Url { get; set; }

    [JsonPropertyName(nameof(IsDownloaded))]
    public bool IsDownloaded { get; set; }

    [JsonPropertyName(nameof(Tags))]
    public IEnumerable<string> Tags { get; set; }
}
