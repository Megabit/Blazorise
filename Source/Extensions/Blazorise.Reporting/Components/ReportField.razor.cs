using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Declares a data-bound field element in a report band.
/// </summary>
public partial class ReportField : ReportElementBase
{
    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Field;

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        var definition = base.BuildDefinition();
        definition.Field = Field;
        definition.Format = Format;
        return definition;
    }

    /// <summary>
    /// Field name or expression resolved against the current data item.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Format string applied to the resolved field value.
    /// </summary>
    [Parameter] public string Format { get; set; }
}