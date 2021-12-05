#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Notifies the component of file upload progress.
    /// </summary>
    public interface IFileEntryNotifier
    {
        /// <summary>
        /// Notifies the component that file upload is about to start.
        /// </summary>
        /// <param name="fileEntry">File entry to be uploaded.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateFileStartedAsync( IFileEntry fileEntry );

        /// <summary>
        /// Notifies the component that file upload has ended.
        /// </summary>
        /// <param name="fileEntry">Uploaded file entry.</param>
        /// <param name="success">True if the file upload was successful.</param>
        /// <param name="fileInvalidReason">Provides information about the invalid file.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateFileEndedAsync( IFileEntry fileEntry, bool success, FileInvalidReason fileInvalidReason );

        /// <summary>
        /// Updates component with the latest file data.
        /// </summary>
        /// <param name="fileEntry">Currently processed file entry.</param>
        /// <param name="position">The current position of this stream.</param>
        /// <param name="data">Currerntly read data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateFileWrittenAsync( IFileEntry fileEntry, long position, byte[] data );

        /// <summary>
        /// Updated the component with the latest upload progress.
        /// </summary>
        /// <param name="fileEntry">Currently processed file entry.</param>
        /// <param name="progressProgress">Progress value.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateFileProgressAsync( IFileEntry fileEntry, long progressProgress );
    }
}
