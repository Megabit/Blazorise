using System.Text.Json.Serialization;

namespace Blazorise.Reporting;

/// <summary>
/// Defines the base contract for user-friendly report value formats.
/// </summary>
[JsonConverter( typeof( ReportFormatDefinitionJsonConverter ) )]
public abstract class ReportFormatDefinition
{
    /// <summary>
    /// Formatting category used for the value.
    /// </summary>
    public abstract ReportFormatCategory Category { get; }

    /// <summary>
    /// Optional culture name used for formatting.
    /// </summary>
    public string CultureName { get; set; }
}