namespace Blazorise
{
    /// <summary>
    /// Context for File Picker's Files
    /// </summary>
    public class FilePickerFileContext
    {
        /// <summary>
        /// Default context constructor.
        /// </summary>
        /// <param name="fileEntry">The File Entry.</param>
        public FilePickerFileContext( IFileEntry fileEntry )
        {
            File = fileEntry;
        }

        /// <summary>
        /// Gets the File Entry.
        /// </summary>
        public IFileEntry File { get; }
    }
}