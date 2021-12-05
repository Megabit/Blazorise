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

        public RemoteFileEntryStreamReader( IJSFileModule jsModule, ElementReference elementRef, FileEntry fileEntry, IFileEntryNotifier fileEntryNotifier, int maxMessageSize )
            : base( jsModule, elementRef, fileEntry, fileEntryNotifier )
        {
            this.maxMessageSize = maxMessageSize;
        }

        public async Task WriteToStreamAsync( Stream stream, CancellationToken cancellationToken )
        {
            await FileEntryNotifier.UpdateFileStartedAsync( FileEntry );

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
                        await FileEntryNotifier.UpdateFileEndedAsync( FileEntry, false, FileInvalidReason.MaxLengthExceeded );
                        return;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    await stream.WriteAsync( buffer, cancellationToken );

                    position += buffer.Length;

                    // notify of all the changes
                    await Task.WhenAll(
                        FileEntryNotifier.UpdateFileWrittenAsync( FileEntry, position, buffer ),
                        FileEntryNotifier.UpdateFileProgressAsync( FileEntry, buffer.Length ) );
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                var isSuccess = position == FileEntry.Size;
                await FileEntryNotifier.UpdateFileEndedAsync( FileEntry, isSuccess, isSuccess
                    ? FileInvalidReason.None
                    : FileInvalidReason.UnexpectedError );
            }
        }
    }
}
