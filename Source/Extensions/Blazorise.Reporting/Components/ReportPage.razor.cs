#region Using directives
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a named report design page and the bands it contains.
/// </summary>
public partial class ReportPage : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ReportPageDefinition page = new()
        {
            Name = Name,
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

        PageContext = new( page );
        ReportContext?.RegisterPage( page );
    }

    #endregion

    #region Properties

    internal ReportPageContext PageContext { get; private set; }

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Friendly page name shown in the designer.
    /// </summary>
    [Parameter] public string Name { get; set; }

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

    /// <summary>
    /// Declarative report bands placed on this page.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}