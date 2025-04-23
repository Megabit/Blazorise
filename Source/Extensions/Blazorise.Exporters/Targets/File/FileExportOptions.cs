namespace Blazorise.Exporters;

/// <summary>
/// Base class for file export options, providing default values for file name and MIME type.
/// </summary>
public abstract class FileExportOptions : IFileExportOptions
{
    /// <summary>
    /// Represents the name of a file without its extension. Defaults to 'exported-data'.
    /// </summary>
    public string FileNameNoExtension { get; init; } = "exported-data";

    /// <summary>
    /// Represents the file extension associated with a file.
    /// </summary>
    public abstract string FileExtension { get; init; }

    /// <summary>
    /// Represents the MIME type of a resource, initialized to 'application/text'.
    /// </summary>
    public virtual string MimeType { get; init; } = "application/text";
}