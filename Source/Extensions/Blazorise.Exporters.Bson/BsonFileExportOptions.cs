namespace Blazorise.Exporters.Bson;

/// <summary>
/// Options for exporting files in BSON format, including file extension and MIME type.
/// </summary>
public class BsonFileExportOptions : FileExportOptions
{
    /// <summary>
    /// Represents the file extension for the object, initialized to 'bson'.
    /// </summary>
    public override string FileName{ get; init; } = "exported-data.bson";

    /// <summary>
    /// Represents the MIME type for BSON data format. It is initialized to 'application/bson'.
    /// </summary>
    public override string MimeType { get; init; } = "application/bson";

    /// <summary>
    /// Indicates whether type information should be included. Defaults to true.
    /// </summary>
    public bool IncludeTypeInformation { get; init; } = true;
}