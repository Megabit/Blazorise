namespace Blazorise.PivotGrid;

/// <summary>
/// Helper class used to override default PivotGrid cell styling.
/// </summary>
public class PivotGridCellStyling
{
    /// <summary>
    /// Cell custom class names.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Cell custom inline styles.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Cell variant color.
    /// </summary>
    public Color Color { get; set; } = Color.Default;

    /// <summary>
    /// Cell background utility color.
    /// </summary>
    public Background Background { get; set; } = Background.Default;

    /// <summary>
    /// Cell custom text color.
    /// </summary>
    public TextColor TextColor { get; set; } = TextColor.Default;

    /// <summary>
    /// Cell text weight.
    /// </summary>
    public TextWeight TextWeight { get; set; } = TextWeight.Default;
}