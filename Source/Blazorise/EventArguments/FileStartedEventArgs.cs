#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provides the information about the file started to be uploaded.
    /// </summary>
    public class FileStartedEventArgs : EventArgs
    {
        public FileStartedEventArgs( IFileEntry file )
        {
            File = file;
        }

        /// <summary>
        /// Gets the file currently being uploaded.
        /// </summary>
        public IFileEntry File { get; }
    }
}
