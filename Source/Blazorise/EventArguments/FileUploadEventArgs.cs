#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Provides the file context for upload.
/// </summary>
public class FileUploadEventArgs : EventArgs
{
    /// <summary>
    /// Gets the file currently being uploaded.
    /// </summary>
    public IFileEntry File { get; }

    /// <summary>
    /// A default <see cref="FileUploadEventArgs"/> constructor.
    /// </summary>
    /// <param name="file">File that is being processed.</param>
    public FileUploadEventArgs( IFileEntry file )
    {
        File = file;
    }
}