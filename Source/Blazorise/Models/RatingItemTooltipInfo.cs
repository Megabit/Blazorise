namespace Blazorise;

/// <summary>
/// Defines the tooltip information for the <see cref="Rating"/> component.
/// </summary>
public class RatingItemTooltipInfo
{
    /// <summary>
    /// Initializes a new instance of the RatingItemTooltipInfo class with specified tooltip text.
    /// The tooltip is placed at the top by default.
    /// </summary>
    /// <param name="text">The text to display in the tooltip.</param>
    public RatingItemTooltipInfo( string text )
        : this( text, TooltipPlacement.Top )
    {
    }

    /// <summary>
    /// Initializes a new instance of the RatingItemTooltipInfo class with specified tooltip text and placement.
    /// </summary>
    /// <param name="text">The text to display in the tooltip.</param>
    /// <param name="placement">The placement of the tooltip relative to its component.</param>
    public RatingItemTooltipInfo( string text, TooltipPlacement placement )
    {
        Text = text;
        Placement = placement;
    }

    /// <summary>
    /// Initializes a new instance of the RatingItemTooltipInfo class with specified tooltip text, placement, multiline option, and arrow visibility.
    /// </summary>
    /// <param name="text">The text to display in the tooltip.</param>
    /// <param name="placement">The placement of the tooltip relative to its component.</param>
    /// <param name="multiline">Determines if the tooltip text should be displayed in multiple lines.</param>
    /// <param name="showArrow">Determines if the tooltip should show an arrow pointing to the component.</param>
    public RatingItemTooltipInfo( string text, TooltipPlacement placement, bool multiline, bool showArrow )
    {
        Text = text;
        Placement = placement;
        Multiline = multiline;
        ShowArrow = showArrow;
    }

    /// <summary>
    /// Gets the tooltip's content text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the tooltip's placement relative to the Rating component it is associated with.
    /// </summary>
    public TooltipPlacement Placement { get; }

    /// <summary>
    /// Gets a value indicating whether the tooltip text is displayed across multiple lines.
    /// </summary>
    public bool Multiline { get; } = false;

    /// <summary>
    /// Gets a value indicating whether the tooltip displays an arrow pointing towards the Rating component.
    /// </summary>
    public bool ShowArrow { get; } = true;
}
