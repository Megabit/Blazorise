#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the horizontal report designer ruler.
/// </summary>
public partial class _ReportDesignerHorizontalRuler
{
    #region Members

    private readonly ReportDesignerRulerService rulerService = new();

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( ( parameters.TryGetValue<double>( nameof( Width ), out double paramWidth ) && paramWidth != Width )
             || ( parameters.TryGetValue<double>( nameof( WidthOffset ), out double paramWidthOffset ) && paramWidthOffset != WidthOffset ) )
        {
            DirtyStyles();
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildClasses( Blazorise.Utilities.ClassBuilder builder )
    {
        builder.Append( "b-report-designer-ruler" );
        builder.Append( "b-report-designer-ruler-horizontal" );
    }

    /// <inheritdoc />
    protected override void BuildStyles( Blazorise.Utilities.StyleBuilder builder )
    {
        builder.Append( $"width:calc(var(--b-report-designer-ruler-surface-offset) + {ReportMeasurementConverter.ToCssPixelString( Width + WidthOffset )})" );
    }

    private string GetTickClass( ReportDesignerRulerTick tick )
    {
        return tick.Major
            ? "b-report-designer-ruler-tick b-report-designer-ruler-tick-major"
            : "b-report-designer-ruler-tick";
    }

    private string GetTickStyle( ReportDesignerRulerTick tick )
    {
        return $"left:calc(var(--b-report-designer-ruler-surface-offset) + {ReportMeasurementConverter.ToCssPixelString( WidthOffset + tick.Position )})";
    }

    private string GetMarkerClass()
    {
        return Marker?.Active == true
            ? "b-report-designer-ruler-marker b-report-designer-ruler-marker-active"
            : "b-report-designer-ruler-marker";
    }

    private string GetMarkerStyle( double position )
    {
        return $"left:calc(var(--b-report-designer-ruler-surface-offset) + {ReportMeasurementConverter.ToCssPixelString( WidthOffset + position )})";
    }

    private string GetMarkerRangeClass()
    {
        return Marker?.Active == true
            ? "b-report-designer-ruler-marker-range b-report-designer-ruler-marker-range-active"
            : "b-report-designer-ruler-marker-range";
    }

    private string GetMarkerRangeStyle()
    {
        return $"left:calc(var(--b-report-designer-ruler-surface-offset) + {ReportMeasurementConverter.ToCssPixelString( WidthOffset + ( Marker?.X ?? 0 ) )});width:{ReportMeasurementConverter.ToCssPixelString( Marker?.Width ?? 0 )}";
    }

    #endregion

    #region Properties

    private IReadOnlyList<ReportDesignerRulerTick> Ticks => rulerService.BuildTicks( Unit, Width, ShowFineTicks );

    private bool HasMarker => Marker is not null;

    /// <summary>
    /// Unit used by the report page.
    /// </summary>
    [Parameter] public ReportMeasurementUnit Unit { get; set; }

    /// <summary>
    /// Ruler width in report layout units.
    /// </summary>
    [Parameter] public double Width { get; set; }

    /// <summary>
    /// Left offset reserved by designer chrome inside the page.
    /// </summary>
    [Parameter] public double WidthOffset { get; set; }

    /// <summary>
    /// Shows fine-grained ruler ticks.
    /// </summary>
    [Parameter] public bool ShowFineTicks { get; set; }

    /// <summary>
    /// Current ruler marker.
    /// </summary>
    [Parameter] public ReportDesignerRulerMarker Marker { get; set; }

    #endregion
}