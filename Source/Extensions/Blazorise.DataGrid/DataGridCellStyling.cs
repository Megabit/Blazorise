namespace Blazorise.DataGrid;

/// <summary>
/// Helpers class to override default cell styling.
/// </summary>
public class DataGridCellStyling
{
    /// <summary>
    /// Row custom class names.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Row custom styles.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Cell custom color.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Cell custom background color.
    /// </summary>
    public Background Background { get; set; }

    /// <summary>
    /// Cell custom text color.
    /// </summary>
    public TextColor TextColor { get; set; }
}
