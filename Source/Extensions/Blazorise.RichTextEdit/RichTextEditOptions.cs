namespace Blazorise.RichTextEdit
{
    /// <summary>
    /// Blazorise RichTextEdit extension options
    /// </summary>
    public sealed class RichTextEditOptions
    {
        /// <summary>
        /// Load the QuillJs snow theme related resources
        /// </summary>
        public bool UseShowTheme { get; set; } = true;

        /// <summary>
        /// Load the QuillJs bubble theme related resources
        /// </summary>
        public bool UseBubbleTheme { get; set; }

        /// <summary>
        /// The QuillJs version to load
        /// </summary>
        public string QuillJsVersion { get; set; } = "1.3.7";

        /// <summary>
        /// Load the RichTextEdit scripts and stylesheets on demand.
        /// </summary>
        public bool DynamicLoadReferences { get; set; } = true;
    }
}