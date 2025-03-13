#region Using directives
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Owner object used to handle the file streaming.
/// </summary>
public interface IFileEntryOwner
{
    /// <summary>
    /// Writes the file data to the target stream.
    /// </summary>
    /// <param name="fileEntry">Currently processed file entry.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="cancellationToken">A cancellation token to signal the cancellation of streaming file data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task WriteToStreamAsync( IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default );

    /// <summary>
    /// Opens the stream for reading the uploaded file.
    /// </summary>
    /// <param name="fileEntry">Currently processed file entry.</param>
    /// <param name="cancellationToken">A cancellation token to signal the cancellation of streaming file data.</param>
    /// <returns>Returns the stream for the uploaded file entry.</returns>
    Stream OpenReadStream( IFileEntry fileEntry, CancellationToken cancellationToken = default );

    /// <summary>
    /// Removes the file entry from js dictionary.
    /// </summary>
    /// <param name="fileEntry">Currently processed file entry.</param>
    /// <param name="cancellationToken">A cancellation token to signal the cancellation of streaming file data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    async Task RemoveFileEntry( IFileEntry fileEntry, CancellationToken cancellationToken = default )
    {
        await JSFileModule.RemoveFileEntry( ElementRef, fileEntry.Id );
    }

    /// <summary>
    /// Element reference.
    /// </summary>
    ElementReference ElementRef { get; set; }

    /// <summary>
    /// JS file module.
    /// </summary>    
    IJSFileModule JSFileModule { get; set; }
}