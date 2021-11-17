#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Middleman between the FileEdit component and javascript.
    /// </summary>
    public class FileEditAdapter
    {
        private readonly IFileEdit fileEdit;

        /// <summary>
        /// Default constructor for <see cref="FileEditAdapter"/>.
        /// </summary>
        /// <param name="fileEdit">File input to which the adapter is referenced.</param>
        public FileEditAdapter( IFileEdit fileEdit )
        {
            this.fileEdit = fileEdit;
        }

        /// <summary>
        /// Notify us from JS that file(s) has changed.
        /// </summary>
        /// <param name="files">List of changed files.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [JSInvokable]
        public Task NotifyChange( FileEntry[] files )
        {
            return fileEdit.NotifyChange( files );
        }
    }
}
