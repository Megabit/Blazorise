#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Padding options.
/// </summary>
public class ChartPadding
{
    public ChartPadding()
    {
    }

    public ChartPadding( double? all )
    {
        Top = Right = Bottom = Left = all;
    }

    public ChartPadding( double? x, double? y )
    {
        Left = Right = x;
        Top = Bottom = y;
    }

    public ChartPadding( double? top, double? right, double? bottom, double? left )
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }

    /// <summary>
    /// Padding from top in pixels.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Top { get; set; }

    /// <summary>
    /// Padding from right in pixels.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Right { get; set; }

    /// <summary>
    /// Padding from bottom in pixels.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Bottom { get; set; }

    /// <summary>
    /// Padding from left in pixels.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Left { get; set; }
}