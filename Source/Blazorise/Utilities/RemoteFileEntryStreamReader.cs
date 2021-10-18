#region Using directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Utilities
{
    internal class RemoteFileEntryStreamReader : FileEntryStreamReader
    {
        private readonly int maxMessageSize;

        public RemoteFileEntryStreamReader( IJSFileEditModule jsModule, ElementReference elementRef, FileEntry fileEntry, FileEdit fileEdit, int maxMessageSize )
            : base( jsModule, elementRef, fileEntry, fileEdit )
        {
            this.maxMessageSize = maxMessageSize;
        }

        public async Task WriteToStreamAsync( Stream stream, CancellationToken cancellationToken )
        {
            await FileEdit.UpdateFileStartedAsync( FileEntry );

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
                        throw new InvalidOperationException( $"Requested a maximum of {length}, but received {buffer.Length}" );
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    await stream.WriteAsync( buffer, cancellationToken );

                    position += buffer.Length;

                    // notify of all the changes
                    await Task.WhenAll(
                        FileEdit.UpdateFileWrittenAsync( FileEntry, position, buffer ),
                        FileEdit.UpdateFileProgressAsync( FileEntry, buffer.Length ) );
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                await FileEdit.UpdateFileEndedAsync( FileEntry, position == FileEntry.Size );
            }
        }
    }
}
