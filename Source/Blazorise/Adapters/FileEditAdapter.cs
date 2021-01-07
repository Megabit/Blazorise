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

        public FileEditAdapter( IFileEdit fileEdit )
        {
            this.fileEdit = fileEdit;
        }

        [JSInvokable]
        public Task NotifyChange( FileEntry[] files )
        {
            return fileEdit.NotifyChange( files );
        }
    }
}
