#region Using directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Helper class for handling the read and write of uploaded file.
    /// </summary>
    public class FileEntry : IFileEntry
    {
        #region Methods

        /// <summary>
        /// Initialized the <see cref="FileEntry"/> object.
        /// </summary>
        /// <param name="owner">File-entry parent component.</param>
        public void Init( IFileEntryOwner owner )
        {
            Owner = owner;
        }

        /// <inheritdoc/>
        public async Task WriteToStreamAsync( Stream stream )
        {
            await Owner.WriteToStreamAsync( this, stream );
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Gets or sets the file-entry parent component.
        /// </summary>
        public IFileEntryOwner Owner { get; set; }

        /// <summary>
        /// Gets or sets the file-entry id.
        /// </summary>
        public int Id { get; set; }

        /// <inheritdoc/>
        public DateTime LastModified { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public long Size { get; set; }

        /// <inheritdoc/>
        public string Type { get; set; }

        /// <inheritdoc/>
        public string UploadUrl { get; set; }

        /// <inheritdoc/>
        public string ErrorMessage { get; set; }

        #endregion
    }
}
