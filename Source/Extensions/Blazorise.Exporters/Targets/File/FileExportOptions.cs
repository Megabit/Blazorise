namespace Blazorise.Exporters;

/// <summary>
/// Defines configuration options for exporting DataGrid content to a file, including file name, extension, and MIME type.
/// </summary>
public interface IFileExportOptions
{
    public string FileName => $"{FileNameNoExtension}.{FileExtension}";
    public  string FileNameNoExtension { get; init; }
    public  string FileExtension { get; init; } 
    public  string MimeType { get; init; }
}

/// <summary>
/// Base class for file export options, providing default values for file name and MIME type.
/// </summary>
public abstract class FileExportOptions:IFileExportOptions
{
    public  string FileNameNoExtension { get; init; } = "exported-data";
    public abstract string FileExtension { get; init; }
    public virtual string MimeType { get; init; } = "application/text";
}

