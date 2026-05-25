#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG bubble chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgBubbleChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    public SvgBubbleChart()
    {
        Type = SvgChartType.Bubble;
    }

    #endregion
}