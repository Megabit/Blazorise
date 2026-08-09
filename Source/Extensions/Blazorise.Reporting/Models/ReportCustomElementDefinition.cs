#region Using directives
using System.Text.Json.Nodes;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a report element supplied by a registered plugin.
/// </summary>
public sealed class ReportCustomElementDefinition : ReportElementDefinition
{
    #region Properties

    /// <inheritdoc />
    public override ReportElementType Type => ReportElementType.Custom;

    /// <summary>
    /// Stable plugin type name used to locate the element plugin.
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// Version of the plugin-owned properties schema.
    /// </summary>
    public int SchemaVersion { get; set; } = 1;

    /// <summary>
    /// Plugin-owned serializable element properties.
    /// </summary>
    public JsonObject Properties { get; set; } = new();

    #endregion
}