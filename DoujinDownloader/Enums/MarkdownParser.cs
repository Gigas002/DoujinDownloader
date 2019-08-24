namespace DoujinDownloader.Enums
{
    /// <summary>
    /// Some stuff for .md file in input to parse.
    /// </summary>
    internal static class MarkdownParser
    {
        /// <summary>
        /// [name](link), language
        /// </summary>
        internal static string DoujinNamePattern { get; } = @"^\[(?'name'.*)\]\((?'link'.*)\)\,?\ ?(?'language'.*)";

        /// <summary>
        /// [name](link)
        /// </summary>
        internal static string ArtistNamePattern { get; } = @"^## \[(?'name'.*)\]\((?'link'.*)\)";

        /// <summary>
        /// "- [x] "
        /// </summary>
        internal static string CheckBoxChecked { get; } = "- [x] ";

        /// <summary>
        /// "- [ ] "
        /// </summary>
        internal static string CheckBoxUnchecked { get; } = "- [ ] ";

        /// <summary>
        /// "# "
        /// </summary>
        internal static string HeaderPattern { get; } = "# ";

        /// <summary>
        /// "## "
        /// </summary>
        internal static string ArtistPattern { get; } = "## ";

        /// <summary>
        /// "### "
        /// </summary>
        internal static string SubsectionPattern { get; } = "### ";
    }
}
