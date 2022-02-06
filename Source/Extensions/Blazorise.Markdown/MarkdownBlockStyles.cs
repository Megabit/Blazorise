namespace Blazorise.Markdown
{
    /// <summary>
    /// Customize how certain buttons that style blocks of text behave.
    /// </summary>
    public class MarkdownBlockStyles
    {
        /// <summary>
        /// Can be set to ** or __.
        /// Defaults to **.
        /// </summary>
        public string Bold { get; set; } = "**";

        /// <summary>
        /// Can be set to ``` or ~~~.
        /// Defaults to ```.
        /// </summary>
        public string Code { get; set; } = "```";

        /// <summary>
        /// Can be set to * or _.
        /// Defaults to *.
        /// </summary>
        public string Italic { get; set; } = "*";
    }
}
