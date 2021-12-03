namespace DoujinDownloader;

public class MarkdownParser
{
    #region Constants

    /// <summary>
    /// Downloaded doujin line starting symbols
    /// </summary>
    private const string Checked = "- [x]";

    /// <summary>
    /// Artist and circle name line starting symbols
    /// </summary>
    private const string ArtistPattern = "## ";

    private const char TagSeparator = ';';

    #endregion

    public static IEnumerable<Doujin> ParseMarkdownAsync(IEnumerable<string> markdownLines)
    {
        var doujins = new List<Doujin>();

        DoujinAuthor author = null;
        string subsection = null;

        const string subsectionShortStart = "###";
        const string doujinShortStart = "- [";

        // Skip empty lines
        var lines = markdownLines.Where(markdownLine => !string.IsNullOrWhiteSpace(markdownLine));

        foreach (var markdownLine in lines)
        {
            var lineStart = markdownLine[..3];

            switch (lineStart)
            {
                case ArtistPattern:
                {
                    author = ParseAuthorLine(markdownLine);

                    // if new artist = mark subsection empty
                    subsection = null;

                    break;
                }
                case subsectionShortStart:
                {
                    subsection = markdownLine[4..].Trim();

                    break;
                }
                case doujinShortStart:
                {
                    var doujin = ParseDoujinLine(markdownLine, author, subsection);
                    doujins.Add(doujin);

                    break;
                }
            }
        }

        return doujins;
    }

    public static Doujin ParseDoujinLine(string doujinLine, DoujinAuthor author, string subsection = null)
    {
        var checkBox = doujinLine[..5];
        var isDownloaded = checkBox == Checked;

        var djLine = doujinLine.Split(checkBox)[1];
        var splitByTagSeparator = djLine.Split(TagSeparator);

        string[] tags = null;

        if (splitByTagSeparator.Length == 2) tags = splitByTagSeparator[1].Split(",").Select(t => t.Trim()).ToArray();

        var splitByUrlBlock = splitByTagSeparator[0].Split("](");
        var urlAndLanguage = splitByUrlBlock[1].Replace(")", string.Empty).Split(',');
        var url = urlAndLanguage[0].Trim();

        string lang = null;
        if (urlAndLanguage.Length == 2) lang = urlAndLanguage[1].Trim();

        var name = splitByUrlBlock[0].Trim()[1..];

        return new Doujin
        {
            Author = author,
            Language = lang,
            IsDownloaded = isDownloaded,
            Name = name,
            Subsection = subsection,
            Tags = tags,
            Url = string.IsNullOrWhiteSpace(url) ? null : new Uri(url)
        };
    }

    public static DoujinAuthor ParseAuthorLine(string authorLine)
    {
        var splitByTagSeparator = authorLine.Split(TagSeparator);

        string[] tags = null;
        if (splitByTagSeparator.Length == 2) tags = splitByTagSeparator[1].Split(",").Select(t => t.Trim()).ToArray();

        var splitByUrlBlock = splitByTagSeparator[0].Split("](");
        var url = splitByUrlBlock[1].Replace(")", string.Empty);
        var namesAndCircles = splitByUrlBlock[0].Remove(0, 4);
        var circlesStartIndex = namesAndCircles.IndexOf('\'');

        string[] names;
        string[] circles = null;

        if (circlesStartIndex <= 0)
        {
            names = namesAndCircles.Split(',');
        }
        else
        {
            names = namesAndCircles[..circlesStartIndex].Split(',');
            circles = namesAndCircles[circlesStartIndex..].Replace("\'", string.Empty).Split(',');
        }

        return new DoujinAuthor
        {
            Names = names,
            Circles = circles,
            Url = string.IsNullOrWhiteSpace(url) ? null : new Uri(url),
            Tags = tags
        };
    }
}
