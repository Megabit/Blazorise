#region Using directives
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Helper class for handling the read and write of uploaded file.
/// </summary>
public class FileEntry : IFileEntry
{
    #region Members

    CancellationTokenSource writeToStreamcancellationTokenSource;
    CancellationTokenSource openReadStreamcancellationTokenSource;

    #endregion

    #region Methods

    /// <summary>
    /// Initialized the <see cref="FileEntry"/> object.
    /// </summary>
    /// <param name="owner">File-entry parent component.</param>
    public void Init( IFileEntryOwner owner )
    {
        Owner = owner;
    }

    /// <inheritdoc/>
    public async Task WriteToStreamAsync( Stream stream, CancellationToken cancellationToken = default )
    {
        writeToStreamcancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
        await Owner.WriteToStreamAsync( this, stream, writeToStreamcancellationTokenSource.Token );
    }

    /// <inheritdoc/>
    public Stream OpenReadStream( long maxAllowedSize = 512000, CancellationToken cancellationToken = default )
    {
        openReadStreamcancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
        if ( Size > maxAllowedSize )
        {
            throw new IOException( $"Supplied file with size {Size} bytes exceeds the maximum of {maxAllowedSize} bytes." );
        }

        return Owner.OpenReadStream( this, openReadStreamcancellationTokenSource.Token );
    }

    /// <inheritdoc/>
    public Task Cancel()
    {
        writeToStreamcancellationTokenSource?.Cancel();
        openReadStreamcancellationTokenSource?.Cancel();

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the file-entry parent component.
    /// </summary>
    [JsonIgnore]
    public IFileEntryOwner Owner { get; set; }

    /// <summary>
    /// Gets or sets the file-entry id.
    /// </summary>
    public int Id { get; set; }

    /// <inheritdoc/>
    public DateTime LastModified { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string RelativePath { get; set; }

    /// <inheritdoc/>
    public long Size { get; set; }

    /// <inheritdoc/>
    public string Type { get; set; }

    /// <inheritdoc/>
    public string UploadUrl { get; set; }

    /// <inheritdoc/>
    public string ErrorMessage { get; set; }

    /// <inheritdoc/>
    public FileEntryStatus Status { get; set; }

    /// <summary>
    /// Provides a completion source to delay completion until after the file operation has been fully completed.
    /// </summary>
    [JsonIgnore]
    public TaskCompletionSource FileUploadEndedCallback { get; set; }

    #endregion
}