#region Using directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    internal class RemoteFileEntryStreamReader : FileEntryStreamReader
    {
        private readonly int maxMessageSize;
        private readonly long maxFileSize;

        public RemoteFileEntryStreamReader( IJSFileModule jsModule, ElementReference elementRef, FileEntry fileEntry, IFileEntryNotifier fileEntryNotifier, int maxMessageSize, long maxFileSize )
            : base( jsModule, elementRef, fileEntry, fileEntryNotifier )
        {
            this.maxMessageSize = maxMessageSize;
            this.maxFileSize = maxFileSize;
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

                    var length = Math.Min( maxMessageSize, FileEntry.Size - position );

                    var base64 = await JSModule.ReadDataAsync( ElementRef, FileEntry.Id, position, length, cancellationToken );
                    var buffer = Convert.FromBase64String( base64 );

                    if ( length != buffer.Length )
                    {
                        await FileEntryNotifier.UpdateFileEndedAsync( FileEntry, false, FileInvalidReason.UnexpectedBufferChunkLength );
                        return;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    await stream.WriteAsync( buffer, cancellationToken );

                    position += buffer.Length;

                    await Task.WhenAll(
                        FileEntryNotifier.UpdateFileWrittenAsync( FileEntry, position, buffer ),
                        FileEntryNotifier.UpdateFileProgressAsync( FileEntry, buffer.Length ) );
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
            }
        }
    }
}
