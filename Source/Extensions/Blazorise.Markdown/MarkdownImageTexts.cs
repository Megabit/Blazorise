namespace Blazorise.Markdown
{
    /// <summary>
    /// Texts displayed to the user (mainly on the status bar) for the import image feature,
    /// where #image_name#, #image_size# and #image_max_size# will replaced by their respective values,
    /// that can be used for customization or internationalization
    /// </summary>
    public class MarkdownImageTexts
    {
        /// <summary>
        /// Status message displayed initially if uploadImage is set to true. Defaults to Attach files by drag
        /// and dropping or pasting from clipboard.
        /// </summary>
        public string Init { get; set; }

        /// <summary>
        /// Status message displayed when the user drags a file to the text area. Defaults to Drop image to upload it.
        /// </summary>
        public string OnDragEnter { get; set; }

        /// <summary>
        /// Status message displayed when the user drops a file in the text area. Defaults to Uploading images #images_names#.
        /// </summary>
        public string OnDrop { get; set; }

        /// <summary>
        /// Status message displayed to show uploading progress. Defaults to Uploading #file_name#: #progress#%.
        /// </summary>
        public string Progress { get; set; }

        /// <summary>
        /// Status message displayed when the image has been uploaded. Defaults to Uploaded #image_name#.
        /// </summary>
        public string OnUploaded { get; set; }

        /// <summary>
        /// A comma-separated list of units used to display messages with human-readable file sizes. Defaults
        /// to B, KB, MB (example: 218 KB). You can use B,KB,MB instead if you prefer without whitespaces (218KB).
        /// </summary>
        public string SizeUnits { get; set; }
    }
}
