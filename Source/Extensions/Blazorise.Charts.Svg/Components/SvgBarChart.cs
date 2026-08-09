#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG bar chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgBarChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    /// <summary>
    /// Creates a Cartesian chart with horizontal value bars.
    /// </summary>
    public SvgBarChart()
    {
        Type = SvgChartType.Bar;
    }

    #endregion
}