#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the upload file.
    /// </summary>
    public interface IFileEntry
    {
        /// <summary>
        /// Returns the last modified time of the file.
        /// </summary>
        DateTime LastModified { get; }

        /// <summary>
        /// Returns the name of the file.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the size of the file in bytes.
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Returns the MIME type of the file.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Provides the access to the underline file through the stream.
        /// </summary>
        /// <param name="stream">Stream to which the upload process if writing.</param>
        /// <returns></returns>
        Task WriteToStreamAsync( Stream stream );
    }
}
