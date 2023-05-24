using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Context for File Picker's Files
/// </summary>
public class FilePickerFileContext
{
    /// <summary>
    /// Gets the RemoveFile event.
    /// </summary>
    private EventCallback<IFileEntry> RemoveFileEntry { get; }

    /// <summary>
    /// Default context constructor.
    /// </summary>
    /// <param name="fileEntry">The File Entry.</param>
    /// <param name="removeFile">The RemoveFile event.</param>
    public FilePickerFileContext( IFileEntry fileEntry, EventCallback<IFileEntry> removeFile )
    {
        File = fileEntry;
        RemoveFileEntry = removeFile;
    }

    /// <summary>
    /// Gets the File Entry.
    /// </summary>
    public IFileEntry File { get; }


    /// <summary>
    /// Triggers the RemoveFile event.
    /// </summary>
    public Task RemoveFile()
        => RemoveFileEntry.InvokeAsync( File );
}