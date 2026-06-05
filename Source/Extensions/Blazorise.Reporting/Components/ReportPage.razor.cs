using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Declares page size and orientation for a report.
/// </summary>
public partial class ReportPage : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( ReportContext is not null )
        {
            ReportContext.RegisterPage( Page );
        }
    }

    /// <summary>
    /// Named page size used when explicit dimensions are not supplied.
    /// </summary>
    [Parameter] public ReportPageSize Size { get; set; } = ReportPageSize.A4;

    /// <summary>
    /// Page orientation applied to the selected size.
    /// </summary>
    [Parameter] public ReportOrientation Orientation { get; set; } = ReportOrientation.Portrait;

    /// <summary>
    /// Explicit page width in designer units.
    /// </summary>
    [Parameter] public double? Width { get; set; }

    /// <summary>
    /// Explicit page height in designer units.
    /// </summary>
    [Parameter] public double? Height { get; set; }

    internal ReportPageDefinition Page => new()
    {
        Size = Size,
        Orientation = Orientation,
        Width = Width ?? 0,
        Height = Height ?? 0,
    };
}