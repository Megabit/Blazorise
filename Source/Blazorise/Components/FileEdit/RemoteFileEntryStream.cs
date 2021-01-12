using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise
{
    internal class RemoteFileEntryStream : Stream
    {
        private readonly IJSRunner _jsRunner;
        private readonly ElementReference _elementRef;
        private readonly FileEntry _fileEntry;
        private readonly FileEdit _fileEdit;
        private readonly int _maxMessageSize;
        private readonly TimeSpan _segmentFetchTimeout;
        private readonly PipeReader _pipeReader;
        private readonly CancellationTokenSource _fillBufferCts;
        private bool _isReadingCompleted;
        private bool _isDisposed;
        private long _position;

        public RemoteFileEntryStream( IJSRunner jsRunner, ElementReference elementRef, FileEntry fileEntry, FileEdit fileEdit, int maxMessageSize, TimeSpan segmentFetchTimeout, CancellationToken cancellationToken )
        {
            _jsRunner = jsRunner;
            _elementRef = elementRef;
            _fileEntry = fileEntry;
            _fileEdit = fileEdit;
            _maxMessageSize = maxMessageSize;
            _segmentFetchTimeout = segmentFetchTimeout;

            var pipe = new Pipe( new PipeOptions( pauseWriterThreshold: _maxMessageSize, resumeWriterThreshold: _maxMessageSize ) );
            _pipeReader = pipe.Reader;
            _fillBufferCts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );

            _ = FillBuffer( pipe.Writer, _fillBufferCts.Token );
        }

        private async Task FillBuffer( PipeWriter writer, CancellationToken cancellationToken )
        {
            long offset = 0;

            try
            {
                while ( offset < _fileEntry.Size )
                {
                    var pipeBuffer = writer.GetMemory( _maxMessageSize );

                    try
                    {
                        using var readSegmentCts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
                        readSegmentCts.CancelAfter( _segmentFetchTimeout );

                        var length = (int)Math.Min( _maxMessageSize, _fileEntry.Size - offset );
                        var base64 = await _jsRunner.ReadDataAsync( cancellationToken, _elementRef, _fileEntry.Id, offset, length );
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
                            _fileEdit.UpdateFileWrittenAsync( _fileEntry, offset, bytes ),
                            _fileEdit.UpdateFileProgressAsync( _fileEntry, bytes.Length ) );

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
                await _fileEdit.UpdateFileEndedAsync( _fileEntry, offset == _fileEntry.Size );
            }

            await writer.CompleteAsync();
        }

        protected async ValueTask<int> CopyFileDataIntoBuffer( long sourceOffset, Memory<byte> destination, CancellationToken cancellationToken )
        {
            if ( _isReadingCompleted )
            {
                return 0;
            }

            int totalBytesCopied = 0;

            while ( destination.Length > 0 )
            {
                var result = await _pipeReader.ReadAsync( cancellationToken );
                var bytesToCopy = (int)Math.Min( result.Buffer.Length, destination.Length );

                if ( bytesToCopy == 0 )
                {
                    if ( result.IsCompleted )
                    {
                        _isReadingCompleted = true;
                        await _pipeReader.CompleteAsync();
                    }

                    break;
                }

                var slice = result.Buffer.Slice( 0, bytesToCopy );
                slice.CopyTo( destination.Span );

                _pipeReader.AdvanceTo( slice.End );

                totalBytesCopied += bytesToCopy;
                destination = destination.Slice( bytesToCopy );
            }

            return totalBytesCopied;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _fileEntry.Size;

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
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

            var bytesRead = await CopyFileDataIntoBuffer( _position, buffer.Slice( 0, maxBytesToRead ), cancellationToken );

            _position += bytesRead;

            return bytesRead;
        }

        protected override void Dispose( bool disposing )
        {
            if ( _isDisposed )
            {
                return;
            }

            _fillBufferCts.Cancel();

            _isDisposed = true;

            base.Dispose( disposing );
        }
    }
}