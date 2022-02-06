namespace Blazorise.Markdown
{
    /// <summary>
    /// Adjust settings for parsing the Markdown during previewing (not editing)
    /// </summary>
    public class MarkdownRenderingConfig
    {
        /// <summary>
        /// If set to true, will highlight using highlight.js.
        /// Defaults to false.
        /// 
        /// To use this feature you must include highlight.js on your page or pass in using the hljs option.
        /// For example, include the script and the CSS files like:
        /// &lt;script src = "https://cdn.jsdelivr.net/highlight.js/latest/highlight.min.js"&gt;&lt;/script&gt;
        /// &lt;link rel="stylesheet" href="https://cdn.jsdelivr.net/highlight.js/latest/styles/github.min.css"&gt;
        /// </summary>
        public bool CodeSyntaxHighlighting { get; set; }

        /// <summary>
        /// An injectible instance of highlight.js.
        /// If you don't want to rely on the global namespace (window.hljs), you can provide an instance here.
        /// Defaults to undefined.
        /// </summary>
        public string Hljs { get; set; }

        /// <summary>
        /// Set the internal Markdown renderer's options <see href="https://marked.js.org/using_advanced#options"/>.
        /// Other renderingConfig options will take precedence.
        /// </summary>
        public object MarkedOptions { get; set; }

        /// <summary>
        /// If set to false, disable parsing GFM single line breaks.
        /// Defaults to true.
        /// </summary>
        public bool SingleLineBreaks { get; set; } = true;

        // TODO: add sanitizerFunction option (function has to return string, not a Promise)
    }
}
