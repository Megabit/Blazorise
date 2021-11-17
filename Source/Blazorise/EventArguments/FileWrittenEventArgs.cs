#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the data being written while uploading.
    /// </summary>
    public class FileWrittenEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="FileWrittenEventArgs"/> constructor.
        /// </summary>
        /// <param name="file">File that is being read.</param>
        /// <param name="position">Read offset in bytes within the file.</param>
        /// <param name="data">Bytes that are being read.</param>
        public FileWrittenEventArgs( IFileEntry file, long position, byte[] data )
        {
            File = file;
            Position = position;
            Data = data;
        }

        /// <summary>
        /// Gets the file currently being uploaded.
        /// </summary>
        public IFileEntry File { get; }

        /// <summary>
        /// Gets the current position offset based on the original data source.
        /// </summary>
        public long Position { get; }

        /// <summary>
        /// Gets the data buffer.
        /// </summary>
        public byte[] Data { get; }
    }
}
