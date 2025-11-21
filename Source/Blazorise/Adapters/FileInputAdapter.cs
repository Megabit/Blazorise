#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Middleman between the FileInput component and javascript.
/// </summary>
public class FileInputAdapter
{
    private readonly IFileInput fileInput;

    /// <summary>
    /// Default constructor for <see cref="FileInputAdapter"/>.
    /// </summary>
    /// <param name="fileInput">File input to which the adapter is referenced.</param>
    public FileInputAdapter( IFileInput fileInput )
    {
        this.fileInput = fileInput;
    }

    /// <summary>
    /// Notify us from JS that file(s) has changed.
    /// </summary>
    /// <param name="files">List of changed files.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyChange( FileEntry[] files )
    {
        return fileInput.NotifyChange( files );
    }
}