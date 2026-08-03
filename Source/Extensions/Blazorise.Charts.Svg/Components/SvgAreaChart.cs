#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG area chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgAreaChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    /// <summary>
    /// Creates a Cartesian chart with filled areas beneath line series.
    /// </summary>
    public SvgAreaChart()
    {
        Type = SvgChartType.Area;
    }

    #endregion
}