#region Using directives
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    internal class RemoteFileEntryStream : Stream
    {
        #region Members

        private readonly IJSRunner jsRunner;
        private readonly ElementReference elementRef;
        private readonly FileEntry fileEntry;
        private readonly FileEdit fileEdit;
        private readonly int maxMessageSize;
        private readonly TimeSpan segmentFetchTimeout;
        private readonly PipeReader pipeReader;
        private readonly CancellationTokenSource fillBufferCts;
        private bool disposed;
        private long position;
        private bool isReadingCompleted;

        #endregion

        #region Constructors

        public RemoteFileEntryStream( IJSRunner jsRunner, ElementReference elementRef, FileEntry fileEntry, FileEdit fileEdit, int maxMessageSize, TimeSpan segmentFetchTimeout, CancellationToken cancellationToken )
        {
            this.jsRunner = jsRunner;
            this.elementRef = elementRef;
            this.fileEntry = fileEntry;
            this.fileEdit = fileEdit;
            this.maxMessageSize = maxMessageSize;
            this.segmentFetchTimeout = segmentFetchTimeout;
            fillBufferCts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
            var pipe = new Pipe( new PipeOptions( pauseWriterThreshold: this.maxMessageSize, resumeWriterThreshold: this.maxMessageSize ) );
            pipeReader = pipe.Reader;

            _ = FillBuffer( pipe.Writer, fillBufferCts.Token );
        }

        #endregion

        #region Methods

        private async Task FillBuffer( PipeWriter writer, CancellationToken cancellationToken )
        {
            long offset = 0;

            try
            {
                while ( offset < fileEntry.Size )
                {
                    var pipeBuffer = writer.GetMemory( maxMessageSize );

                    try
                    {
                        using var readSegmentCts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
                        readSegmentCts.CancelAfter( segmentFetchTimeout );

                        var length = (int)Math.Min( maxMessageSize, fileEntry.Size - offset );
                        var base64 = await jsRunner.ReadDataAsync( cancellationToken, elementRef, fileEntry.Id, offset, length );
                        var bytes = Convert.FromBase64String( base64 );

                        if ( bytes is null || bytes.Length != length )
                        {
                            throw new InvalidOperationException(
                                $"A segment with size {bytes?.Length ?? 0} bytes was received, but {length} bytes were expected." );
                        }

                        bytes.CopyTo( pipeBuffer );
                        writer.Advance( length );
                        offset += length;

                        var result = await writer.FlushAsync( cancellationToken );

                        await Task.WhenAll(
                            fileEdit.UpdateFileWrittenAsync( fileEntry, offset, bytes ),
                            fileEdit.UpdateFileProgressAsync( fileEntry, bytes.Length ) );

                        if ( result.IsCompleted )
                        {
                            break;
                        }
                    }
                    catch ( Exception e )
                    {
                        await writer.CompleteAsync( e );
                        return;
                    }
                }
            }
            finally
            {
                await fileEdit.UpdateFileEndedAsync( fileEntry, offset == fileEntry.Size );
            }

            await writer.CompleteAsync();
        }

        protected async ValueTask<int> CopyFileDataIntoBuffer( long sourceOffset, Memory<byte> destination, CancellationToken cancellationToken )
        {
            if ( isReadingCompleted )
            {
                return 0;
            }

            int totalBytesCopied = 0;

            while ( destination.Length > 0 )
            {
                var result = await pipeReader.ReadAsync( cancellationToken );
                var bytesToCopy = (int)Math.Min( result.Buffer.Length, destination.Length );

                if ( bytesToCopy == 0 )
                {
                    if ( result.IsCompleted )
                    {
                        isReadingCompleted = true;
                        await pipeReader.CompleteAsync();
                    }

                    break;
                }

                var slice = result.Buffer.Slice( 0, bytesToCopy );
                slice.CopyTo( destination.Span );

                pipeReader.AdvanceTo( slice.End );

                totalBytesCopied += bytesToCopy;
                destination = destination.Slice( bytesToCopy );
            }

            return totalBytesCopied;
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

        public override Task<int> ReadAsync( byte[] buffer, int offset, int count, CancellationToken cancellationToken )
            => ReadAsync( new Memory<byte>( buffer, offset, count ), cancellationToken ).AsTask();

        public override async ValueTask<int> ReadAsync( Memory<byte> buffer, CancellationToken cancellationToken = default )
        {
            int maxBytesToRead = (int)( Length - Position );

            if ( maxBytesToRead > buffer.Length )
            {
                maxBytesToRead = buffer.Length;
            }

            if ( maxBytesToRead <= 0 )
            {
                return 0;
            }
            var bytesRead = await CopyFileDataIntoBuffer( position, buffer.Slice( 0, maxBytesToRead ), cancellationToken );
            position += bytesRead;

            return bytesRead;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            fillBufferCts.Cancel();

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
}