namespace Blazorise.Markdown
{
    /// <summary>
    /// Adjust settings for parsing the Markdown during editing (not previewing).
    /// </summary>
    public class MarkdownParsingConfig
    {
        /// <summary>
        /// If set to true, will render headers without a space after the #.
        /// Defaults to false.
        /// </summary>
        public bool AllowAtxHeaderWithoutSpace { get; set; }

        /// <summary>
        /// If set to false, will not process GFM strikethrough syntax.
        /// Defaults to true.
        /// </summary>
        public bool Strikethrough { get; set; } = true;
    }
}
