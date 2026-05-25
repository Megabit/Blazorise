#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG radar chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgRadarChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    public SvgRadarChart()
    {
        Type = SvgChartType.Radar;
    }

    #endregion
}