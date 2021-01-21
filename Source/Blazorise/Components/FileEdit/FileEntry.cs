#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public class FileEntry : IFileEntry
    {
        #region Methods

        public void Init( FileEdit fileEdit )
        {
            Owner = fileEdit;
        }

        public async Task WriteToStreamAsync( Stream stream )
        {
            await Owner.WriteToStreamAsync( this, stream );
        }

        public Stream OpenReadStream( long maxAllowedSize = 512000, CancellationToken cancellationToken = default )
        {
            if ( Size > maxAllowedSize )
            {
                throw new IOException( $"Supplied file with size {Size} bytes exceeds the maximum of {maxAllowedSize} bytes." );
            }

            return Owner.OpenReadStream( this, cancellationToken );
        }

        #endregion

        #region Properties

        internal FileEdit Owner { get; set; }

        public int Id { get; set; }

        public DateTime LastModified { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public string Type { get; set; }

        #endregion
    }
}
