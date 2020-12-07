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
