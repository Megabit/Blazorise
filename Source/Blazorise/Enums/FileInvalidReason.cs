#region Using directives
#endregion

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
        /// Unexpected error, please see exception.
        /// </summary>
        UnexpectedError
    } 
}
