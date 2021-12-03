using System.Globalization;
using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global

namespace DoujinDownloader.HitomiDownloader;

/// <summary>
/// Image metadata from hitomi.la's response
/// </summary>
public class ImageInfo
{
    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    [JsonPropertyName("haswebp")]
    public byte HasWebp { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("hasavif")]
    public byte HasAvif { get; set; }

    internal Uri ImageUri { get; set; }

    internal FileInfo ImageFileInfo { get; set; }

    public ImageInfo(int width, string hash, byte hasWebp, string name, int height, byte hasAvif)
    {
        Width = width;
        Hash = hash;
        HasWebp = hasWebp;
        Name = name;
        Height = height;
        HasAvif = hasAvif;

        ImageFileInfo = new FileInfo(Name);

        ImageUrlFromImageInfo();
    }

    private void ImageUrlFromImageInfo()
    {
        string h1 = Hash[^1..];
        string h2 = Hash[^3..^1];

        long frontendNumber = 3;
        string subdomain = "a";

        long.TryParse(h2, NumberStyles.HexNumber, null, out long g);

        if (g < 0x30) frontendNumber = 2;
        if (g < 0x09) g = 1;

        subdomain = $"{(char)(97 + g % frontendNumber)}{subdomain}";

        string directory = "images";

        //if (HasWebp == 1)
        //{
        //    directory = "webp";
        //    ImageFileInfo = new FileInfo(Path.ChangeExtension(ImageFileInfo.Name, ".webp"));
        //}

        ImageUri = new Uri($"https://{subdomain}.hitomi.la/{directory}/{h1}/{h2}/{Hash}{ImageFileInfo.Extension}");
    }
}
