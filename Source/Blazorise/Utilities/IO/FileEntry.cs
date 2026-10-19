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

    private long size;

    private byte[] data;

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
        EnsureHasContent();

        writeToStreamcancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken, UploadCancellationToken );

        if ( HasSuppliedContent )
        {
            var maxAllowedSize = UploadNotifier is FileInput input ? input.MaxFileSize : long.MaxValue;
            await using var source = CreateSuppliedReadStream( maxAllowedSize, writeToStreamcancellationTokenSource.Token );
            var bufferSize = UploadNotifier is FileInput fileInput ? fileInput.MaxChunkSize : 81920;

            await source.CopyToAsync( stream, bufferSize, writeToStreamcancellationTokenSource.Token );
            return;
        }

        await Owner.WriteToStreamAsync( this, stream, writeToStreamcancellationTokenSource.Token );
    }

    /// <inheritdoc/>
    public Stream OpenReadStream( long maxAllowedSize = 512000, CancellationToken cancellationToken = default )
    {
        EnsureHasContent();

        openReadStreamcancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken, UploadCancellationToken );
        if ( Size > maxAllowedSize )
        {
            throw new IOException( $"Supplied file with size {Size} bytes exceeds the maximum of {maxAllowedSize} bytes." );
        }

        return HasSuppliedContent
            ? CreateSuppliedReadStream( maxAllowedSize, openReadStreamcancellationTokenSource.Token )
            : Owner.OpenReadStream( this, openReadStreamcancellationTokenSource.Token );
    }

    private Stream CreateSuppliedReadStream( long maxAllowedSize, CancellationToken cancellationToken )
    {
        var suppliedData = Data;
        var streamFactory = StreamFactory;

        if ( suppliedData is not null )
        {
            streamFactory = _ => Task.FromResult<Stream>( new MemoryStream( suppliedData, writable: false ) );
        }

        var maxChunkSize = UploadNotifier is FileInput fileInput ? fileInput.MaxChunkSize : 81920;
        var effectiveMaxAllowedSize = UploadNotifier is FileInput input ? Math.Min( maxAllowedSize, input.MaxFileSize ) : maxAllowedSize;
        var notifier = UploadNotifier is FileInput { DisableProgressReport: true } ? null : UploadNotifier;

        return new FileEntryReadStream( streamFactory, this, notifier, effectiveMaxAllowedSize, maxChunkSize, cancellationToken );
    }

    /// <inheritdoc/>
    public Task Cancel()
    {
        writeToStreamcancellationTokenSource?.Cancel();
        openReadStreamcancellationTokenSource?.Cancel();

        return Task.CompletedTask;
    }

    private void EnsureHasContent()
    {
        if ( Data is not null && StreamFactory is not null )
        {
            throw new InvalidOperationException( "Supply either Data or StreamFactory for a file entry, not both." );
        }

        if ( !HasContent )
        {
            throw new InvalidOperationException( "The file entry has no available content. Supply Data or StreamFactory for cached files, or select a file in the browser. Metadata alone does not provide file data." );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Tracks whether the browser still retains this entry's file data.
    /// </summary>
    internal bool HasBrowserFileData { get; set; }

    /// <summary>
    /// Gets whether the entry has supplied bytes or an application-provided stream source.
    /// </summary>
    internal bool HasSuppliedContent => Data is not null || StreamFactory is not null;

    /// <summary>
    /// Gets whether the entry can supply its content.
    /// </summary>
    internal bool HasContent => HasSuppliedContent || ( Owner is not null && ( Owner is not FileInput || HasBrowserFileData ) );

    /// <summary>
    /// Gets or sets the notifier used while the picker uploads supplied content.
    /// </summary>
    internal IFileEntryNotifier UploadNotifier { get; set; }

    /// <summary>
    /// Gets or sets the cancellation token for the current picker upload.
    /// </summary>
    internal CancellationToken UploadCancellationToken { get; set; }

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
    public long Size
    {
        get => Data?.LongLength ?? size;
        set => size = value;
    }

    /// <summary>
    /// Gets or sets optional file contents, for example bytes restored from a local cache.
    /// When supplied, these bytes are used by the streaming methods and determine <see cref="Size"/>.
    /// Browser-selected files are not automatically loaded into this property.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public byte[] Data
    {
        get => data;
        set
        {
            data = value;

            if ( data is not null )
            {
                size = data.LongLength;
            }
        }
    }

    /// <summary>
    /// Gets or sets an asynchronous factory that opens a fresh readable stream for each read or upload attempt.
    /// Use this instead of <see cref="Data"/> to stream large cached files without loading all their bytes into memory.
    /// Set <see cref="Size"/> to the expected content length. Returned streams are disposed by the entry's read stream.
    /// The factory is not serialized and must be assigned again after restoring cached file details.
    /// </summary>
    [JsonIgnore]
    public Func<CancellationToken, Task<Stream>> StreamFactory { get; set; }

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