#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    internal class RemoteFileEntryStreamReader : FileEntryStreamReader
    {
        private readonly int messageSize;
        private readonly int bufferSize;

        public RemoteFileEntryStreamReader( IJSRunner jsRunner, ElementReference elementRef, FileEntry fileEntry, FileEdit fileEdit, int messageSize, int bufferSize )
            : base( jsRunner, elementRef, fileEntry, fileEdit )
        {
            this.messageSize = messageSize;
            this.bufferSize = bufferSize;
        }

        public async Task WriteToStreamAsync( Stream stream, CancellationToken cancellationToken )
        {
            await fileEdit.UpdateProgressAsync( fileEntry, 0, 0, fileEntry.Size );

            long position = 0;
            long queuePosition = 0;

            try
            {
                var queue = new Queue<ValueTask<string>>();

                while ( position < fileEntry.Size )
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    while ( queue.Count < bufferSize && queuePosition < fileEntry.Size )
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var taskPosition = queuePosition;
                        var taskSize = Math.Min( messageSize, ( fileEntry.Size - queuePosition ) );

                        var task = jsRunner.ReadDataAsync( cancellationToken, elementRef, fileEntry.Id, taskPosition, taskSize );

                        queue.Enqueue( task );
                        queuePosition += taskSize;

                        await fileEdit.UpdateProgressAsync( fileEntry, 0, taskSize, 0 );
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    if ( queue.Count == 0 )
                    {
                        continue;
                    }

                    var task2 = queue.Dequeue();

                    var base64 = await task2.ConfigureAwait( true );
                    var buffer = Convert.FromBase64String( base64 );

                    await stream.WriteAsync( buffer, cancellationToken );

                    position += buffer.Length;

                    // notify of all the changes
                    await Task.WhenAll(
                        fileEdit.UpdateWrittenAsync( fileEntry, position, buffer ),
                        fileEdit.UpdateProgressAsync( fileEntry, buffer.Length, 0, 0 ) );
                }
            }
            finally
            {
                await fileEdit.UpdateProgressAsync( fileEntry, -position, -queuePosition, -fileEntry.Size );
            }
        }
    }
}
