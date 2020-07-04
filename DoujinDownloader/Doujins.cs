using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DoujinDownloader
{
    /// <summary>
    /// Json root.
    /// </summary>
    public sealed class Doujins
    {
        /// <summary>
        /// List of <see cref="Doujin"/> objects.
        /// </summary>
        [JsonPropertyName(nameof(Doujins))]
        public List<Doujin> DoujinsList { get; set; } = new List<Doujin>();
    }
}
