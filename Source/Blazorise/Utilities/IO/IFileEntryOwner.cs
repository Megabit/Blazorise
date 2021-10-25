﻿#region Using directives
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Owner object used to handle the file streaming.
    /// </summary>
    public interface IFileEntryOwner
    {
        /// <summary>
        /// Writes the file data to the target stream.
        /// </summary>
        /// <param name="fileEntry">Currently processed file entry.</param>
        /// <param name="stream">Target stream.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task WriteToStreamAsync( FileEntry fileEntry, Stream stream );

        /// <summary>
        /// Opens the stream for reading the uploaded file.
        /// </summary>
        /// <param name="fileEntry">Currently processed file entry.</param>
        /// <param name="cancellationToken">A cancellation token to signal the cancellation of streaming file data.</param>
        /// <returns>Returns the stream for the uploaded file entry.</returns>
        Stream OpenReadStream( FileEntry fileEntry, CancellationToken cancellationToken = default );
    }
}
