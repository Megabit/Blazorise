namespace Blazorise.DataGrid;

/// <summary>
/// Defines row expand rendering options for <see cref="DataGrid{TItem}"/>.
/// </summary>
public class DataGridExpandOptions
{
    /// <summary>
    /// Default indentation size in pixels.
    /// </summary>
    public const int DefaultIndentSize = 16;

    /// <summary>
    /// Default expand icon.
    /// </summary>
    public const IconName DefaultExpandIcon = IconName.ChevronRight;

    /// <summary>
    /// Default collapse icon.
    /// </summary>
    public const IconName DefaultCollapseIcon = IconName.ChevronDown;

    /// <summary>
    /// Defines indentation size in pixels.
    /// </summary>
    public int IndentSize { get; set; } = DefaultIndentSize;

    /// <summary>
    /// Defines the expand icon.
    /// </summary>
    public IconName ExpandIcon { get; set; } = DefaultExpandIcon;

    /// <summary>
    /// Defines the collapse icon.
    /// </summary>
    public IconName CollapseIcon { get; set; } = DefaultCollapseIcon;
}