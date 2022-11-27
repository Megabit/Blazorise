#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Determines the File Entry Status
/// </summary>
public enum FileEntryStatus
{
    /// <summary>
    /// The file is ready to be uploaded.
    /// </summary>
    Ready,

    /// <summary>
    /// The file is being uploaded.
    /// </summary>
    Uploading,

    /// <summary>
    /// The file has been uploaded.
    /// </summary>
    Uploaded,

    /// <summary>
    /// The file exceeds the maximum configured size.
    /// </summary>
    ExceedsMaximumSize,

    /// <summary>
    /// Something wrong ocorred when trying to upload the file.
    /// </summary>
    Error,
}