#region Using directives
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    internal class RemoteFileEntryStream : Stream
    {
        #region Members

        private readonly IJSFileModule jsModule;
        private readonly ElementReference elementRef;
        private readonly IFileEntry fileEntry;
        private readonly IFileEntryNotifier fileEntryNotifier;
        private readonly int maxMessageSize;
        private readonly TimeSpan segmentFetchTimeout;
        private readonly long maxFileSize;
        private readonly PipeReader pipeReader;
        private readonly CancellationTokenSource fillBufferCts;
        private bool disposed;
        private long position;
        private bool isReadingCompleted;

        #endregion

        #region Constructors

        public RemoteFileEntryStream( IJSFileModule jsModule, ElementReference elementRef, IFileEntry fileEntry, IFileEntryNotifier fileEntryNotifier, int maxMessageSize, TimeSpan segmentFetchTimeout, long maxFileSize, CancellationToken cancellationToken )
        {
            this.jsModule = jsModule;
            this.elementRef = elementRef;
            this.fileEntry = fileEntry;
            this.fileEntryNotifier = fileEntryNotifier;
            this.maxMessageSize = maxMessageSize;
            this.segmentFetchTimeout = segmentFetchTimeout;
            this.maxFileSize = maxFileSize;
            fillBufferCts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
            var pipe = new Pipe( new( pauseWriterThreshold: this.maxMessageSize, resumeWriterThreshold: this.maxMessageSize ) );
            pipeReader = pipe.Reader;

            _ = FillBuffer( pipe.Writer, fillBufferCts.Token );
        }

        #endregion

        #region Methods

        private async Task FillBuffer( PipeWriter writer, CancellationToken cancellationToken )
        {
            if ( maxFileSize < fileEntry.Size )
            {
                await fileEntryNotifier.UpdateFileEndedAsync( fileEntry, false, FileInvalidReason.MaxLengthExceeded );
                return;
            }

            long position = 0;

            try
            {
                while ( position < fileEntry.Size )
                {
                    var pipeBuffer = writer.GetMemory( maxMessageSize );

                    try
                    {
                        using var readSegmentCts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
                        readSegmentCts.CancelAfter( segmentFetchTimeout );

                        var length = (int)Math.Min( maxMessageSize, fileEntry.Size - position );
                        var base64 = await jsModule.ReadDataAsync( elementRef, fileEntry.Id, position, length, cancellationToken );
                        var bytes = Convert.FromBase64String( base64 );

                        if ( bytes is null || bytes.Length != length )
                        {
                            await fileEntryNotifier.UpdateFileEndedAsync( fileEntry, false, FileInvalidReason.UnexpectedBufferChunkLength );
                            await writer.CompleteAsync();
                            return;
                        }

                        bytes.CopyTo( pipeBuffer );
                        writer.Advance( length );
                        position += length;

                        var result = await writer.FlushAsync( cancellationToken );

                        await Task.WhenAll(
                            fileEntryNotifier.UpdateFileWrittenAsync( fileEntry, position, bytes ),
                            fileEntryNotifier.UpdateFileProgressAsync( fileEntry, bytes.Length ) );

                        if ( result.IsCompleted )
                        {
                            break;
                        }
                    }
                    catch ( OperationCanceledException oce )
                    {
                        await writer.CompleteAsync( oce );
                        await fileEntryNotifier.UpdateFileEndedAsync( fileEntry, false, FileInvalidReason.TaskCancelled );
                        return;
                    }
                    catch ( Exception e )
                    {
                        await writer.CompleteAsync( e );
                    }
                }
            }
            finally
            {
                if ( !cancellationToken.IsCancellationRequested )
                {
                    var success = position == fileEntry.Size;
                    var overMaxBufferChunkLength = position > fileEntry.Size;
                    var fileInvalidReason = success
                        ? FileInvalidReason.None
                        : overMaxBufferChunkLength
                            ? FileInvalidReason.UnexpectedBufferChunkLength
                            : FileInvalidReason.UnexpectedError;

                    await fileEntryNotifier.UpdateFileEndedAsync( fileEntry, success, fileInvalidReason );
                }
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
            => ReadAsync( new( buffer, offset, count ), cancellationToken ).AsTask();

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

            disposed = true;

            fillBufferCts.Cancel();
            fillBufferCts.Dispose();

            base.Dispose( disposing );
        }

        public override ValueTask DisposeAsync()
        {
            if ( disposed )
            {
                return ValueTask.CompletedTask;
            }

            disposed = true;

            fillBufferCts.Cancel();
            fillBufferCts.Dispose();

            return base.DisposeAsync();
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