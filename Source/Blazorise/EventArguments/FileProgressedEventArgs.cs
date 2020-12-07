#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provides the progress state of uploaded file.
    /// </summary>
    public class FileProgressedEventArgs : EventArgs
    {
        public FileProgressedEventArgs( IFileEntry file, double progress )
        {
            File = file;
            Progress = progress;
        }

        /// <summary>
        /// Gets the file currently being uploaded.
        /// </summary>
        public IFileEntry File { get; }

        /// <summary>
        /// Gets the total progress in the range from 0 to 1.
        /// </summary>
        public double Progress { get; }

        /// <summary>
        /// Gets the total progress in the range from 0 to 100.
        /// </summary>
        public double Percentage => Progress * 100d;
    }
}
