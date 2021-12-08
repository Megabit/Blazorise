namespace Blazorise
{
    /// <summary>
    /// Provides information about the invalid file.
    /// </summary>
    public enum FileInvalidReason
    {
        /// <summary>
        /// File is Valid.
        /// </summary>
        None,

        /// <summary>
        /// File Max Lenght was exceeded.
        /// </summary>
        MaxLengthExceeded,

        /// <summary>
        /// The length of the buffer was not as expected when reading the file into the buffer.
        /// </summary>
        UnexpectedBufferChunkLength,

        /// <summary>
        /// Task was cancelled.
        /// </summary>
        TaskCancelled,

        /// <summary>
        /// Unexpected error, please see exception.
        /// </summary>
        UnexpectedError
    }
}
