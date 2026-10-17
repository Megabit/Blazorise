#region Using directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Tests.TestServices;

/// <summary>
/// Generates content without allocating the complete file or supporting seeking.
/// </summary>
internal sealed class GeneratedReadStream : Stream
{
    #region Members

    private readonly long length;

    private readonly int maxReadSize;

    private readonly TaskCompletionSource disposal = new( TaskCreationOptions.RunContinuationsAsynchronously );

    #endregion

    #region Constructors

    public GeneratedReadStream( long length, int maxReadSize = int.MaxValue )
    {
        this.length = length;
        this.maxReadSize = maxReadSize;
    }

    #endregion

    #region Methods

    public override ValueTask<int> ReadAsync( Memory<byte> buffer, CancellationToken cancellationToken = default )
    {
        ObjectDisposedException.ThrowIf( IsDisposed, this );
        cancellationToken.ThrowIfCancellationRequested();
        MaxRequestedSize = Math.Max( MaxRequestedSize, buffer.Length );

        var bytesRead = (int)Math.Min( Math.Min( buffer.Length, maxReadSize ), length - BytesRead );
        buffer.Span.Slice( 0, bytesRead ).Fill( 42 );
        BytesRead += bytesRead;

        return ValueTask.FromResult( bytesRead );
    }

    public override void Flush()
        => throw new NotSupportedException();

    public override int Read( byte[] buffer, int offset, int count )
        => throw new NotSupportedException();

    public override long Seek( long offset, SeekOrigin origin )
        => throw new NotSupportedException();

    public override void SetLength( long value )
        => throw new NotSupportedException();

    public override void Write( byte[] buffer, int offset, int count )
        => throw new NotSupportedException();

    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            IsDisposed = true;
            disposal.TrySetResult();
        }

        base.Dispose( disposing );
    }

    public override ValueTask DisposeAsync()
    {
        IsDisposedAsynchronously = true;
        Dispose();
        return ValueTask.CompletedTask;
    }

    #endregion

    #region Properties

    public long BytesRead { get; private set; }

    public int MaxRequestedSize { get; private set; }

    public bool IsDisposed { get; private set; }

    public bool IsDisposedAsynchronously { get; private set; }

    public Task Disposed => disposal.Task;

    public override bool CanRead => !IsDisposed;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    #endregion
}