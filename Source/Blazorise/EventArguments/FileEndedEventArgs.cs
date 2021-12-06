#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provides the information about the file ended uploading.
    /// </summary>
    public class FileEndedEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="FileEndedEventArgs"/> constructor.
        /// </summary>
        /// <param name="file">File that is ended.</param>
        /// <param name="success">Result of file end upload.</param>
        /// <param name="fileInvalidReason">Reason for file failure.</param>
        public FileEndedEventArgs( IFileEntry file, bool success, FileInvalidReason fileInvalidReason )
        {
            File = file;
            Success = success;
            FileInvalidReason = fileInvalidReason;
        }

        /// <summary>
        /// Gets the file currently being uploaded.
        /// </summary>
        public IFileEntry File { get; }

        /// <summary>
        /// Gets the value indicating if file has finished successfully.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Provides information about the invalid file.
        /// </summary>
        public FileInvalidReason FileInvalidReason { get; set; }
    }
}
