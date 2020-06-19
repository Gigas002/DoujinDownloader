namespace DoujinDownloader.Constants
{
    /// <summary>
    /// Some stuff for .md file in input to parse.
    /// </summary>
    internal static class MarkdownParser
    {
        /// <summary>
        /// [name](link), language
        /// </summary>
        internal const string DoujinNamePattern = @"^\[(?'name'.*)\]\((?'link'.*)\)\,?\ ?(?'language'.*)";

        /// <summary>
        /// [name](link)
        /// </summary>
        internal const string ArtistNamePattern = @"^## \[(?'name'.*)\]\((?'link'.*)\)";

        /// <summary>
        /// "- [x] "
        /// </summary>
        internal const string CheckBoxChecked = "- [x] ";

        /// <summary>
        /// "- [ ] "
        /// </summary>
        internal const string CheckBoxUnchecked = "- [ ] ";

        /// <summary>
        /// "# "
        /// </summary>
        internal const string HeaderPattern = "# ";

        /// <summary>
        /// "## "
        /// </summary>
        internal const string ArtistPattern = "## ";

        /// <summary>
        /// "### "
        /// </summary>
        internal const string SubsectionPattern = "### ";
    }
}
