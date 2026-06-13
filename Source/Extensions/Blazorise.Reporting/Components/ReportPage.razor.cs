using Blazorise.Reporting.Internal;
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
    /// Unit used by the declarative page dimensions and margins.
    /// </summary>
    [Parameter] public ReportMeasurementUnit MeasurementUnit { get; set; } = ReportMeasurementUnit.Centimeter;

    /// <summary>
    /// Explicit page width in the configured measurement unit.
    /// </summary>
    [Parameter] public double? Width { get; set; }

    /// <summary>
    /// Explicit page height in the configured measurement unit.
    /// </summary>
    [Parameter] public double? Height { get; set; }

    /// <summary>
    /// Left printable page margin in the configured measurement unit.
    /// </summary>
    [Parameter] public double MarginLeft { get; set; }

    /// <summary>
    /// Top printable page margin in the configured measurement unit.
    /// </summary>
    [Parameter] public double MarginTop { get; set; }

    /// <summary>
    /// Right printable page margin in the configured measurement unit.
    /// </summary>
    [Parameter] public double MarginRight { get; set; }

    /// <summary>
    /// Bottom printable page margin in the configured measurement unit.
    /// </summary>
    [Parameter] public double MarginBottom { get; set; }

    internal ReportPageDefinition Page => new()
    {
        Size = Size,
        MeasurementUnit = MeasurementUnit,
        Orientation = Orientation,
        Width = ReportMeasurementConverter.ToPoints( Width, MeasurementUnit ),
        Height = ReportMeasurementConverter.ToPoints( Height, MeasurementUnit ),
        Margins = new()
        {
            Left = ReportMeasurementConverter.ToPoints( MarginLeft, MeasurementUnit ),
            Top = ReportMeasurementConverter.ToPoints( MarginTop, MeasurementUnit ),
            Right = ReportMeasurementConverter.ToPoints( MarginRight, MeasurementUnit ),
            Bottom = ReportMeasurementConverter.ToPoints( MarginBottom, MeasurementUnit ),
        },
    };
}