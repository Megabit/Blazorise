#region Using directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Utilities
{
    internal class RemoteFileEntryStreamReader : FileEntryStreamReader
    {
        private readonly int maxMessageSize;

        public RemoteFileEntryStreamReader( IJSRunner jsRunner, ElementReference elementRef, FileEntry fileEntry, FileEdit fileEdit, int maxMessageSize )
            : base( jsRunner, elementRef, fileEntry, fileEdit )
        {
            this.maxMessageSize = maxMessageSize;
        }

        public async Task WriteToStreamAsync( Stream stream, CancellationToken cancellationToken )
        {
            await fileEdit.UpdateFileStartedAsync( fileEntry );

            long position = 0;

            try
            {
                while ( position < fileEntry.Size )
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var length = Math.Min( maxMessageSize, fileEntry.Size - position );

                    var base64 = await jsRunner.ReadDataAsync( cancellationToken, elementRef, fileEntry.Id, position, length );
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
                        fileEdit.UpdateFileWrittenAsync( fileEntry, position, buffer ),
                        fileEdit.UpdateFileProgressAsync( fileEntry, buffer.Length ) );
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                await fileEdit.UpdateFileEndedAsync( fileEntry, position == fileEntry.Size );
            }
        }
    }
}
