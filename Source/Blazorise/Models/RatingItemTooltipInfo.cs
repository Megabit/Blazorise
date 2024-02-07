namespace Blazorise;

/// <summary>
/// Defines the tooltip information for the <see cref="Rating"/> component.
/// </summary>
public class RatingItemTooltipInfo
{
    public RatingItemTooltipInfo( string text )
        : this( text, TooltipPlacement.Top )
    {
    }

    public RatingItemTooltipInfo( string text, TooltipPlacement placement )
    {
        Text = text;
        Placement = placement;
    }

    public RatingItemTooltipInfo( string text, TooltipPlacement placement, bool multiline, bool showArrow )
    {
        Text = text;
        Placement = placement;
        Multiline = multiline;
        ShowArrow = showArrow;
    }

    /// <summary>
    /// Gets a regular tooltip's content. 
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the tooltip location relative to its component.
    /// </summary>
    public TooltipPlacement Placement { get; }

    /// <summary>
    /// Force the multiline display.
    /// </summary>
    public bool Multiline { get; } = false;

    /// <summary>
    /// Gets the tooltip arrow visibility.
    /// </summary>
    public bool ShowArrow { get; } = true;
}
