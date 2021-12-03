using System.Text.Json.Serialization;

namespace DoujinDownloader;

public class DoujinAuthor
{
    [JsonPropertyName(nameof(Names))]
    public IEnumerable<string> Names { get; set; }

    [JsonPropertyName(nameof(Circles))]
    public IEnumerable<string> Circles { get; set; }

    [JsonPropertyName(nameof(Url))]
    public Uri Url { get; set; }

    [JsonPropertyName(nameof(Tags))]
    public IEnumerable<string> Tags { get; set; }
}
