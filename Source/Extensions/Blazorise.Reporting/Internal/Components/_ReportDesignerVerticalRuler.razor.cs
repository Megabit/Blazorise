#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the vertical report designer ruler.
/// </summary>
public partial class _ReportDesignerVerticalRuler
{
    #region Members

    private readonly ReportDesignerRulerService rulerService = new();

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<double>( nameof( Height ), out double paramHeight ) && paramHeight != Height )
            DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildClasses( Blazorise.Utilities.ClassBuilder builder )
    {
        builder.Append( "b-report-designer-ruler" );
        builder.Append( "b-report-designer-ruler-vertical" );
    }

    /// <inheritdoc />
    protected override void BuildStyles( Blazorise.Utilities.StyleBuilder builder )
    {
        builder.Append( $"height:calc(var(--b-report-designer-ruler-surface-offset) + {ReportMeasurementConverter.ToCssPixelString( Height )})" );
    }

    private string GetTickClass( ReportDesignerRulerTick tick )
    {
        return tick.Major
            ? "b-report-designer-ruler-tick b-report-designer-ruler-tick-major"
            : "b-report-designer-ruler-tick";
    }

    private string GetTickStyle( ReportDesignerRulerTick tick )
    {
        return $"top:calc(var(--b-report-designer-ruler-surface-offset) + {ReportMeasurementConverter.ToCssPixelString( tick.Position )})";
    }

    private string GetMarkerClass()
    {
        return Marker?.Active == true
            ? "b-report-designer-ruler-marker b-report-designer-ruler-marker-active"
            : "b-report-designer-ruler-marker";
    }

    private string GetMarkerStyle( double position )
    {
        return $"top:calc(var(--b-report-designer-ruler-surface-offset) + {ReportMeasurementConverter.ToCssPixelString( position )})";
    }

    private string GetMarkerRangeClass()
    {
        return Marker?.Active == true
            ? "b-report-designer-ruler-marker-range b-report-designer-ruler-marker-range-active"
            : "b-report-designer-ruler-marker-range";
    }

    private string GetMarkerRangeStyle()
    {
        return $"top:calc(var(--b-report-designer-ruler-surface-offset) + {ReportMeasurementConverter.ToCssPixelString( Marker?.Y ?? 0 )});height:{ReportMeasurementConverter.ToCssPixelString( Marker?.Height ?? 0 )}";
    }

    #endregion

    #region Properties

    private IReadOnlyList<ReportDesignerRulerTick> Ticks => rulerService.BuildTicks( Unit, Height, ShowFineTicks );

    private bool HasMarker => Marker is not null;

    /// <summary>
    /// Unit used by the report page.
    /// </summary>
    [Parameter] public ReportMeasurementUnit Unit { get; set; }

    /// <summary>
    /// Ruler height in report layout units.
    /// </summary>
    [Parameter] public double Height { get; set; }

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