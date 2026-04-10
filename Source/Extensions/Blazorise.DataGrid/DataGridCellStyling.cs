namespace Blazorise.DataGrid;

/// <summary>
/// Helper class used to override default cell styling.
/// Applied to the rendered cell element.
/// </summary>
public class DataGridCellStyling
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
    public Color Color { get; set; }

    /// <summary>
    /// Cell background utility color.
    /// </summary>
    public Background Background { get; set; }

    /// <summary>
    /// Cell custom text color.
    /// </summary>
    public TextColor TextColor { get; set; }
}