#region Using directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the upload file.
/// </summary>
public interface IFileEntry
{
    /// <summary>
    /// Gets the file-entry id.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Returns the last modified time of the file.
    /// </summary>
    DateTime LastModified { get; }

    /// <summary>
    /// Returns the name of the file.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns the relative path of the file.
    /// </summary>
    string RelativePath { get; }

    /// <summary>
    /// Returns the size of the file in bytes.
    /// </summary>
    long Size { get; }

    /// <summary>
    /// Returns the MIME type of the file.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Provides a way to tell the location of the uploaded file or image.
    /// </summary>
    string UploadUrl { get; set; }

    /// <summary>
    /// Provides a way to tell if any error happened.
    /// </summary>
    string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the File Entry Status.
    /// </summary>
    public FileEntryStatus Status { get; set; }

    /// <summary>
    /// Provides the access to the underline file through the stream.
    /// </summary>
    /// <param name="stream">Stream to which the upload process if writing.</param>
    /// <param name="cancellationToken">A cancellation token to signal the cancellation of streaming file data.</param>
    /// <returns></returns>
    Task WriteToStreamAsync( Stream stream, CancellationToken cancellationToken = default );

    /// <summary>
    /// Opens the stream for reading the uploaded file.
    /// </summary>
    /// <param name="maxAllowedSize">
    /// The maximum number of bytes that can be supplied by the Stream. Defaults to 500 KB.
    /// <para>
    /// Calling <see cref="M:Blazorise.IFileEntry.OpenReadStream(System.Int64,System.Threading.CancellationToken)" />
    /// will throw if the file's size, as specified by <see cref="P:Blazorize.IFileEntry.Size" /> is larger than
    /// <paramref name="maxAllowedSize" />. By default, if the user supplies a file larger than 500 KB, this method will throw an exception.
    /// </para>
    /// <para>
    /// It is valuable to choose a size limit that corresponds to your use case. If you allow excessively large files, this
    /// may result in excessive consumption of memory or disk/database space, depending on what your code does
    /// with the supplied <see cref="T:System.IO.Stream" />.
    /// </para>
    /// <para>
    /// For Blazor Server in particular, beware of reading the entire stream into a memory buffer unless you have
    /// passed a suitably low size limit, since you will be consuming that memory on the server.
    /// </para>
    /// </param>
    /// <param name="cancellationToken">A cancellation token to signal the cancellation of streaming file data.</param>
    /// <exception cref="T:System.IO.IOException">Thrown if the file's length exceeds the <paramref name="maxAllowedSize" /> value.</exception>
    Stream OpenReadStream( long maxAllowedSize = 512000, CancellationToken cancellationToken = default );

    /// <summary>
    /// Cancels any ongoing FileEntry operation.
    /// </summary>
    public Task Cancel();

    /// <summary>
    /// Provides a completion source to delay completion until after the file operation has been fully completed.
    /// </summary>
    TaskCompletionSource FileUploadEndedCallback { get; set; }
}