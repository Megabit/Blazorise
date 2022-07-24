#region Using directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    internal class RemoteFileEntryStreamReader : FileEntryStreamReader
    {
        private readonly int maxMessageSize;
        private readonly long maxFileSize;

        private readonly CancellationTokenSource fillBufferCts;
        private CancellationTokenSource? copyFileDataCts;
        private IJSStreamReference? jsStreamReference;
        private readonly Task<Stream> OpenReadStreamTask;

        public RemoteFileEntryStreamReader( IJSFileModule jsModule, ElementReference elementRef, FileEntry fileEntry, IFileEntryNotifier fileEntryNotifier, int maxMessageSize, long maxFileSize )
            : base( jsModule, elementRef, fileEntry, fileEntryNotifier )
        {
            this.maxMessageSize = maxMessageSize;
            this.maxFileSize = maxFileSize;

            fillBufferCts = new CancellationTokenSource();
            OpenReadStreamTask = OpenReadStreamAsync( fillBufferCts.Token );
        }

        private async Task<Stream> OpenReadStreamAsync( CancellationToken cancellationToken )
        {
            // This method only gets called once, from the constructor, so we're never overwriting an
            // existing _jsStreamReference value
            jsStreamReference = await JSModule.ReadDataAsync( ElementRef, FileEntry.Id, cancellationToken );

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

        public async Task WriteToStreamAsync( Stream stream, CancellationToken cancellationToken )
        {
            await FileEntryNotifier.UpdateFileStartedAsync( FileEntry );

            if ( maxFileSize < FileEntry.Size )
            {
                await FileEntryNotifier.UpdateFileEndedAsync( FileEntry, false, FileInvalidReason.MaxLengthExceeded );
                return;
            }

            long position = 0;

            try
            {
                while ( position < FileEntry.Size )
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var length = (int)Math.Min( maxMessageSize, FileEntry.Size - position );
                    
                    var buffer = new Memory<byte>( new byte[length], 0, length );
                    await CopyFileDataIntoBuffer( buffer, cancellationToken );
                    await stream.WriteAsync( buffer, cancellationToken );

                    cancellationToken.ThrowIfCancellationRequested();

                    position += length;
                    await Task.WhenAll(
                        FileEntryNotifier.UpdateFileWrittenAsync( FileEntry, position, buffer.ToArray() ),
                        FileEntryNotifier.UpdateFileProgressAsync( FileEntry, buffer.Length ) );

                    await RefreshUI();
                }
            }
            catch ( OperationCanceledException )
            {
                await FileEntryNotifier.UpdateFileEndedAsync( FileEntry, false, FileInvalidReason.TaskCancelled );
                throw;
            }
            finally
            {
                if ( !cancellationToken.IsCancellationRequested )
                {
                    var success = position == FileEntry.Size;
                    var overMaxBufferChunkLength = position > FileEntry.Size;
                    var fileInvalidReason = success
                        ? FileInvalidReason.None
                        : overMaxBufferChunkLength
                            ? FileInvalidReason.UnexpectedBufferChunkLength
                            : FileInvalidReason.UnexpectedError;

                    await FileEntryNotifier.UpdateFileEndedAsync( FileEntry, success, fileInvalidReason );
                }


                fillBufferCts.Cancel();
                copyFileDataCts?.Cancel();

                //TODO: Disposal
                try
                {
                    _ = jsStreamReference?.DisposeAsync().Preserve();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// If we're running on WebAssembly, then we should give time for the single thread to catch up and refresh the UI.
        /// </summary>
        /// <returns></returns>
        private Task RefreshUI()
        {
            if ( OperatingSystem.IsBrowser() )
                return Task.Delay( 1 );
            return Task.CompletedTask;
        }
    }


}
