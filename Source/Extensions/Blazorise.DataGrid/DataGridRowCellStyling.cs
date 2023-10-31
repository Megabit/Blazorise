namespace Blazorise.DataGrid;

/// <summary>
/// Helpers class to override default row styling.
/// </summary>
public class DataGridRowCellStyling
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
    /// Row custom color.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Row custom background color.
    /// </summary>
    public Background Background { get; set; }
}