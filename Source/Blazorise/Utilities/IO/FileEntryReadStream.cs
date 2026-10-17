#region Using directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Opens supplied content on demand and reads it using bounded buffers.
/// </summary>
internal sealed class FileEntryReadStream : Stream
{
    #region Members

    private readonly Func<CancellationToken, Task<Stream>> streamFactory;

    private readonly IFileEntry fileEntry;

    private readonly IFileEntryNotifier notifier;

    private readonly long length;

    private readonly int maxChunkSize;

    private readonly CancellationTokenSource lifetimeCancellation;

    private readonly CancellationToken lifetimeToken;

    private readonly byte[] endBuffer = new byte[1];

    private Task<Stream> sourceTask;

    private long position;

    private bool reachedEnd;

    private bool disposed;

    #endregion

    #region Constructors

    public FileEntryReadStream( Func<CancellationToken, Task<Stream>> streamFactory, IFileEntry fileEntry, IFileEntryNotifier notifier, long maxAllowedSize, int maxChunkSize, CancellationToken cancellationToken )
    {
        ArgumentNullException.ThrowIfNull( streamFactory );
        ArgumentOutOfRangeException.ThrowIfNegative( fileEntry.Size );
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero( maxChunkSize );

        if ( fileEntry.Size > maxAllowedSize )
        {
            throw new IOException( $"Supplied file with size {fileEntry.Size} bytes exceeds the maximum of {maxAllowedSize} bytes." );
        }

        this.streamFactory = streamFactory;
        this.fileEntry = fileEntry;
        this.notifier = notifier;
        this.maxChunkSize = maxChunkSize;
        length = fileEntry.Size;
        lifetimeCancellation = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
        lifetimeToken = lifetimeCancellation.Token;
    }

    #endregion

    #region Methods

    public override Task<int> ReadAsync( byte[] buffer, int offset, int count, CancellationToken cancellationToken )
        => ReadAsync( new Memory<byte>( buffer, offset, count ), cancellationToken ).AsTask();

    public override async ValueTask<int> ReadAsync( Memory<byte> buffer, CancellationToken cancellationToken = default )
    {
        ObjectDisposedException.ThrowIf( disposed, this );

        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource( lifetimeToken, cancellationToken );
        linkedCancellation.Token.ThrowIfCancellationRequested();

        if ( buffer.Length == 0 || reachedEnd )
        {
            return 0;
        }

        var bytesRead = await ReadSourceAsync( buffer, linkedCancellation.Token );
        await ReportProgressAsync( buffer.Slice( 0, bytesRead ), linkedCancellation.Token );

        return bytesRead;
    }

    public override async Task CopyToAsync( Stream destination, int bufferSize, CancellationToken cancellationToken )
    {
        ObjectDisposedException.ThrowIf( disposed, this );
        ArgumentNullException.ThrowIfNull( destination );
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero( bufferSize );

        if ( !destination.CanWrite )
        {
            throw new NotSupportedException( "The destination stream does not support writing." );
        }

        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource( lifetimeToken, cancellationToken );
        linkedCancellation.Token.ThrowIfCancellationRequested();

        var buffer = new byte[(int)Math.Min( Math.Min( bufferSize, maxChunkSize ), Math.Max( length - position, 1 ) )];

        while ( !reachedEnd )
        {
            var bytesRead = await ReadSourceAsync( buffer.AsMemory(), linkedCancellation.Token );

            if ( bytesRead > 0 )
            {
                await destination.WriteAsync( buffer.AsMemory( 0, bytesRead ), linkedCancellation.Token );
            }

            await ReportProgressAsync( buffer.AsMemory( 0, bytesRead ), linkedCancellation.Token );
        }
    }

    private async Task<Stream> OpenSourceAsync()
    {
        var source = await streamFactory( lifetimeToken );

        try
        {
            lifetimeToken.ThrowIfCancellationRequested();
            ObjectDisposedException.ThrowIf( disposed, this );

            if ( source is null || !source.CanRead )
            {
                throw new InvalidOperationException( "StreamFactory must return a readable stream." );
            }

            return source;
        }
        catch
        {
            if ( source is not null )
            {
                await source.DisposeAsync();
            }

            throw;
        }
    }

    private async ValueTask<int> ReadSourceAsync( Memory<byte> buffer, CancellationToken cancellationToken )
    {
        sourceTask ??= OpenSourceAsync();
        var source = await sourceTask.WaitAsync( cancellationToken );
        var bytesRead = 0;

        if ( position < length )
        {
            var count = (int)Math.Min( Math.Min( buffer.Length, maxChunkSize ), length - position );
            bytesRead = await source.ReadAsync( buffer.Slice( 0, count ), cancellationToken );

            if ( bytesRead == 0 )
            {
                throw new EndOfStreamException( "StreamFactory returned less content than the declared file Size." );
            }

            if ( bytesRead < 0 || bytesRead > count )
            {
                throw new InvalidDataException( "The source stream returned an invalid number of bytes." );
            }

            position += bytesRead;
        }

        if ( position == length )
        {
            if ( await source.ReadAsync( endBuffer.AsMemory(), cancellationToken ) != 0 )
            {
                throw new InvalidDataException( "StreamFactory returned more content than the declared file Size." );
            }

            reachedEnd = true;
        }

        cancellationToken.ThrowIfCancellationRequested();

        return bytesRead;
    }

    private async Task ReportProgressAsync( Memory<byte> data, CancellationToken cancellationToken )
    {
        if ( notifier is not null )
        {
            await notifier.UpdateFileWrittenAsync( fileEntry, position, data.ToArray() );
            await notifier.UpdateFileProgressAsync( fileEntry, data.Length );
        }

        cancellationToken.ThrowIfCancellationRequested();

        if ( OperatingSystem.IsBrowser() )
        {
            await Task.Delay( 1, cancellationToken );
        }
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
        if ( disposing && !disposed )
        {
            disposed = true;
            lifetimeCancellation.Cancel();

            try
            {
                if ( sourceTask?.IsCompletedSuccessfully == true )
                {
                    sourceTask.Result.Dispose();
                }
                else if ( sourceTask is not null )
                {
                    _ = DisposePendingSourceAsync( sourceTask );
                }
            }
            finally
            {
                lifetimeCancellation.Dispose();
            }
        }

        base.Dispose( disposing );
    }

    public override async ValueTask DisposeAsync()
    {
        if ( disposed )
        {
            return;
        }

        disposed = true;
        lifetimeCancellation.Cancel();

        try
        {
            if ( sourceTask?.IsCompletedSuccessfully == true )
            {
                await sourceTask.Result.DisposeAsync();
            }
            else if ( sourceTask is not null )
            {
                _ = DisposePendingSourceAsync( sourceTask );
            }
        }
        finally
        {
            lifetimeCancellation.Dispose();
            GC.SuppressFinalize( this );
        }
    }

    private static async Task DisposePendingSourceAsync( Task<Stream> sourceTask )
    {
        try
        {
            var source = await sourceTask;
            await source.DisposeAsync();
        }
        catch
        {
            // Opening errors are reported to the reader. Disposal also observes late failures.
        }
    }

    #endregion

    #region Properties

    public override bool CanRead => !disposed;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => length;

    public override long Position
    {
        get => position;
        set => throw new NotSupportedException();
    }

    #endregion
}