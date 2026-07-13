#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Wraps the designer page with ruler chrome.
/// </summary>
public partial class _ReportDesignerRulerLayout
{
    private Task OnReportSelected()
        => ReportSelected.InvokeAsync();

    #region Properties

    /// <summary>
    /// Indicates that ruler chrome should be rendered.
    /// </summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Unit used by the report page.
    /// </summary>
    [Parameter] public ReportMeasurementUnit Unit { get; set; }

    /// <summary>
    /// Page width in report layout units.
    /// </summary>
    [Parameter] public double PageWidth { get; set; }

    /// <summary>
    /// Page height in report layout units.
    /// </summary>
    [Parameter] public double PageHeight { get; set; }

    /// <summary>
    /// Left offset reserved by designer chrome inside the page.
    /// </summary>
    [Parameter] public double WidthOffset { get; set; }

    /// <summary>
    /// Minor grid interval used by ruler ticks.
    /// </summary>
    [Parameter] public double GridSize { get; set; }

    /// <summary>
    /// Shows fine-grained ruler ticks.
    /// </summary>
    [Parameter] public bool ShowFineTicks { get; set; }

    /// <summary>
    /// Current ruler marker.
    /// </summary>
    [Parameter] public ReportDesignerRulerMarker Marker { get; set; }

    /// <summary>
    /// Raised when the report selector corner is clicked.
    /// </summary>
    [Parameter] public EventCallback ReportSelected { get; set; }

    /// <summary>
    /// Designer page content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}