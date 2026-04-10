namespace Blazorise.DataGrid;

/// <summary>
/// Helper class used to override default row styling.
/// Applied to the row <c>tr</c> element.
/// </summary>
public class DataGridRowStyling
{
    /// <summary>
    /// Row custom class names.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Row custom inline styles.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Row variant color.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Row background utility color.
    /// </summary>
    public Background Background { get; set; }
}