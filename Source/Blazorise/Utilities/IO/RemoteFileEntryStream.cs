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
        private readonly CancellationTokenSource fillBufferCts;
        private bool disposed;
        private long position;

        private CancellationTokenSource? _copyFileDataCts;
        private IJSStreamReference? _jsStreamReference;
        private readonly Task<Stream> OpenReadStreamTask;

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

            OpenReadStreamTask = OpenReadStreamAsync( fillBufferCts.Token );
        }

        #endregion

        #region Methods

        private async Task<Stream> OpenReadStreamAsync( CancellationToken cancellationToken )
        {
            // This method only gets called once, from the constructor, so we're never overwriting an
            // existing _jsStreamReference value
            _jsStreamReference = await jsModule.ReadDataAsync( elementRef, fileEntry.Id, cancellationToken );

            return await _jsStreamReference.OpenReadStreamAsync(
                this.maxFileSize,
                cancellationToken: cancellationToken );
        }

        protected async ValueTask<int> CopyFileDataIntoBuffer( Memory<byte> destination, CancellationToken cancellationToken )
        {

            var stream = await OpenReadStreamTask;
            _copyFileDataCts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
            return await stream.ReadAsync( destination, _copyFileDataCts.Token );
        }

        public override Task<int> ReadAsync( byte[] buffer, int offset, int count, CancellationToken cancellationToken )
            => ReadAsync( new( buffer, offset, count ), cancellationToken ).AsTask();

        public override async ValueTask<int> ReadAsync( Memory<byte> buffer, CancellationToken cancellationToken = default )
        {
            if (Position == 0)
                await fileEntryNotifier.UpdateFileStartedAsync( fileEntry );

            int maxBytesToRead = (int)( Length - Position );

            if ( maxBytesToRead > buffer.Length )
            {
                maxBytesToRead = buffer.Length;
            }

            if ( maxBytesToRead <= 0 )
            {
                return 0;
            }

            var bytesRead = await CopyFileDataIntoBuffer( buffer.Slice( 0, maxBytesToRead ), cancellationToken );
            position += bytesRead;

            await Task.WhenAll(
                fileEntryNotifier.UpdateFileWrittenAsync( fileEntry, position, buffer.ToArray() ),
                fileEntryNotifier.UpdateFileProgressAsync( fileEntry, maxBytesToRead ) );

            if (position == fileEntry.Size)
                await fileEntryNotifier.UpdateFileEndedAsync( fileEntry, true, FileInvalidReason.None );

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
            _copyFileDataCts?.Cancel();
            // If the browser connection is still live, notify the JS side that it's free to release the Blob
            // and reclaim the memory. If the browser connection is already gone, there's no way for the
            // notification to get through, but we don't want to fail the .NET-side disposal process for this.
            try
            {
                _ = _jsStreamReference?.DisposeAsync().Preserve();
            }
            catch
            {
            }

            disposed = true;

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