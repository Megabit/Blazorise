#region Using directives
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

internal class RemoteFileEntryStream : Stream
{
    #region Members

    private readonly IJSFileModule jsModule;
    private readonly ElementReference elementRef;
    private readonly IFileEntry fileEntry;
    private readonly IFileEntryNotifier fileEntryNotifier;
    private readonly long maxFileSize;
    private readonly CancellationTokenSource fillBufferCts;
    private bool disposed;
    private long position;

    private CancellationTokenSource copyFileDataCts;
    private IJSStreamReference jsStreamReference;
    private readonly Task<Stream> OpenReadStreamTask;

    #endregion

    #region Constructors

    public RemoteFileEntryStream( IJSFileModule jsModule, ElementReference elementRef, IFileEntry fileEntry, IFileEntryNotifier fileEntryNotifier, long maxFileSize, CancellationToken cancellationToken )
    {
        this.jsModule = jsModule;
        this.elementRef = elementRef;
        this.fileEntry = fileEntry;
        this.fileEntryNotifier = fileEntryNotifier;
        this.maxFileSize = maxFileSize;
        fillBufferCts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );

        OpenReadStreamTask = OpenReadStreamAsync( fillBufferCts.Token );
    }

    #endregion

    #region Methods

    private async Task<Stream> OpenReadStreamAsync( CancellationToken cancellationToken )
    {
        // This method only gets called once, from the constructor, so we're never overwriting an
        // existing _jsStreamReference value
        jsStreamReference = await jsModule.ReadDataAsync( elementRef, fileEntry.Id, cancellationToken );

        return await jsStreamReference.OpenReadStreamAsync(
            this.maxFileSize,
            cancellationToken: cancellationToken );
    }

    protected async ValueTask<int> CopyFileDataIntoBuffer( Memory<byte> destination, CancellationToken cancellationToken )
    {

        var stream = await OpenReadStreamTask;
        copyFileDataCts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
        return await stream.ReadAsync( destination, copyFileDataCts.Token );
    }

    public override Task<int> ReadAsync( byte[] buffer, int offset, int count, CancellationToken cancellationToken )
        => ReadAsync( new( buffer, offset, count ), cancellationToken ).AsTask();
    public override async ValueTask<int> ReadAsync( Memory<byte> buffer, CancellationToken cancellationToken = default )
    {
        var bytesRead = 0;
        try
        {
            if ( Position == 0 )
                await fileEntryNotifier.UpdateFileStartedAsync( fileEntry );

            var bytesAvailableToRead = Length - Position;
            var maxBytesToRead = (int)Math.Min( bytesAvailableToRead, buffer.Length );
            if ( maxBytesToRead <= 0 )
                return 0;

            bytesRead = await CopyFileDataIntoBuffer( buffer.Slice( 0, maxBytesToRead ), cancellationToken );
            position += bytesRead;


            await Task.WhenAll(
                fileEntryNotifier.UpdateFileWrittenAsync( fileEntry, position, buffer.ToArray() ),
                fileEntryNotifier.UpdateFileProgressAsync( fileEntry, bytesRead ) );

            if ( position == fileEntry.Size )
                await fileEntryNotifier.UpdateFileEndedAsync( fileEntry, true, FileInvalidReason.None );

        }
        catch ( OperationCanceledException )
        {
            await fileEntryNotifier.UpdateFileEndedAsync( fileEntry, false, FileInvalidReason.TaskCancelled );
            throw;
        }

        return bytesRead;
    }

    public override void Flush()
        => throw new NotSupportedException();

    public override int Read( byte[] buffer, int offset, int count )
        => throw new NotSupportedException( "Synchronous reads are not supported." );

    public override long Seek( long offset, SeekOrigin origin )
        => throw new NotSupportedException();

    public override void SetLength( long value )
        => throw new NotSupportedException();

    public override void Write( byte[] buffer, int offset, int count )
        => throw new NotSupportedException();



    protected override void Dispose( bool disposing )
    {
        if ( disposed )
        {
            return;
        }

        fillBufferCts.Cancel();
        copyFileDataCts?.Cancel();
        // If the browser connection is still live, notify the JS side that it's free to release the Blob
        // and reclaim the memory. If the browser connection is already gone, there's no way for the
        // notification to get through, but we don't want to fail the .NET-side disposal process for this.
        try
        {
            _ = jsStreamReference?.DisposeAsync().Preserve();
        }
        catch
        {
        }

        disposed = true;

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => fileEntry.Size;

    public override long Position
    {
        get => position;
        set => throw new NotSupportedException();
    }

    #endregion
}