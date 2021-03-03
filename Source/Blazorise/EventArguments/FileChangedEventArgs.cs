#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the selected files ready to be uploaded.
    /// </summary>
    public class FileChangedEventArgs : EventArgs
    {
        public FileChangedEventArgs( IFileEntry[] files )
        {
            Files = files;
        }

        /// <summary>
        /// Gets the list of selected files.
        /// </summary>
        public IFileEntry[] Files { get; }
    }
}
