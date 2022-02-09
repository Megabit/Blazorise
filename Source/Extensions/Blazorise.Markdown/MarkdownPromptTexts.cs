namespace Blazorise.Markdown
{
    /// <summary>
    /// Customize the text used to prompt for URLs.
    /// </summary>
    public class MarkdownPromptTexts
    {
        /// <summary>
        ///  The text to use when prompting for an image's URL.
        ///  Defaults to `URL of the image:`.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The text to use when prompting for a link's URL.
        /// Defaults to `URL for the link:`.
        /// </summary>
        public string Link { get; set; }
    }
}
