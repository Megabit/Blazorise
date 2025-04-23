namespace Blazorise.Exporters;

/// <summary>
/// Defines configuration options for exporting content to a file, including file name, extension, and MIME type.
/// </summary>
public interface IFileExportOptions
{
    /// <summary>
    /// Constructs the full file name by combining the file name without extension and the file extension.
    /// </summary>
    public string FileName => $"{FileNameNoExtension}.{FileExtension}";

    /// <summary>
    /// Represents the name of a file without its extension.
    /// </summary>
    public string FileNameNoExtension { get; init; }

    /// <summary>
    /// Represents the file extension of a file.
    /// </summary>
    public string FileExtension { get; init; }

    /// <summary>
    /// Represents the MIME type of a file or data.
    /// </summary>
    public string MimeType { get; init; }
}