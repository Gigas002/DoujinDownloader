using System.Collections.Generic;
using Newtonsoft.Json;

namespace DoujinDownloader
{
    /// <summary>
    /// Json root.
    /// </summary>
    internal sealed class Doujins
    {
        /// <summary>
        /// List of <see cref="Doujin"/> objects.
        /// </summary>
        [JsonProperty(nameof(Doujins))]
        internal List<Doujin> DoujinsList { get; set; } = new List<Doujin>();
    }
}
