using System.Net;
using System.Text;
using System.Text.Json;
using DoujinDownloader.Constants;

namespace DoujinDownloader.HitomiDownloader;

/// <summary>
/// Class for downloading doujins from hitomi.la
/// </summary>
internal static class HitomiDownloader
{
    /// <summary>
    /// String to replace in respone from, hitomi gallery's .js to get json
    /// </summary>
    private const string ReplaceInResponse = "var galleryinfo = ";

    /// <summary>
    /// Gets hitomi gallery's .js
    /// </summary>
    /// <param name="galleryId">Id of doujin</param>
    /// <returns>Uri</returns>
    private static string GetHitomiGalleryJsString(int galleryId) => $"https://ltn.hitomi.la/galleries/{galleryId}.js";

    /// <summary>
    /// Create <see cref="ReadOnlySpan{T}"/> from byte array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t">Array</param>
    /// <returns></returns>
    private static ReadOnlySpan<T> GetReadOnlySpan<T>(T[] t) => new ReadOnlySpan<T>(t);

    /// <summary>
    /// Downloads one doujin from Hitomi.la
    /// </summary>
    /// <param name="doujinUri">Gallery's uri</param>
    /// <returns></returns>
    internal static async ValueTask DownloadDoujin(Uri doujinUri)
    {
        int galleryId = ParseUri(doujinUri);

        HttpClient client = new HttpClient();
        string response = await client.GetStringAsync(GetHitomiGalleryJsString(galleryId))
                                      .ConfigureAwait(false);

        string json = response.Replace(ReplaceInResponse, string.Empty);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        GalleryInfo gi = JsonSerializer.Deserialize<GalleryInfo>(GetReadOnlySpan(bytes));

        //TODO pass output paths to the method
        DirectoryInfo directoryInfo = new DirectoryInfo($"{galleryId}");
        directoryInfo.Create();

        await DownloadImages(gi?.Images, directoryInfo).ConfigureAwait(false);
    }

    /// <summary>
    /// Get gallery id from hitomi uri
    /// </summary>
    /// <param name="doujinUri">Gallery's uri</param>
    /// <returns>Gallery id</returns>
    private static int ParseUri(Uri doujinUri)
    {
        string lastPartOfUri = doujinUri.AbsolutePath.Split('/')[^1].Replace(Extensions.HtmlExtension, string.Empty);
        string galleryIdString = lastPartOfUri.Split('-')[^1];
        int.TryParse(galleryIdString, out int galleryId);

        return galleryId;
    }

    /// <summary>
    /// Download all images for current doujin
    /// </summary>
    /// <param name="imageInfos">Collection of images to download</param>
    /// <param name="outputDirectoryInfo">Output directory</param>
    /// <returns></returns>
    private static async ValueTask DownloadImages(IEnumerable<ImageInfo> imageInfos, DirectoryInfo outputDirectoryInfo)
    {
        using SemaphoreSlim semaphoreSlim = new SemaphoreSlim(10);
        HashSet<Task> tasks = new HashSet<Task>();

        foreach (ImageInfo imageInfo in imageInfos)
        {
            imageInfo.ImageFileInfo = new FileInfo(Path.Combine(outputDirectoryInfo.FullName, imageInfo.ImageFileInfo.Name));
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);

            try
            {
                tasks.Add(Task.Run(async () => await DownloadImage(imageInfo).ConfigureAwait(false)));
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        foreach (Task task in tasks) task.Dispose();
    }

    /// <summary>
    /// Download one image
    /// </summary>
    /// <param name="imageInfo">Image to download</param>
    /// <returns></returns>
    private static async ValueTask DownloadImage(ImageInfo imageInfo)
    {
        HttpWebRequest httpWebRequest = WebRequest.CreateHttp(imageInfo.ImageUri);
        httpWebRequest.Method = Downloaders.Get;
        httpWebRequest.Headers.Add(HttpRequestHeader.Referer, Downloaders.HitomiLa);

        using HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync().ConfigureAwait(false);
        byte[] buffer = new byte[Downloaders.DefaultBufferSize];

        await using Stream stream = httpWebResponse.GetResponseStream();
        await using FileStream fileStream = imageInfo.ImageFileInfo.OpenWrite();

        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            await fileStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
    }
}
