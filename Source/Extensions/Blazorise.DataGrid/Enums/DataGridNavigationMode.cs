namespace Blazorise.DataGrid;

/// <summary>
/// Defines the options for <see cref="DataGrid{TItem}"/> navigation mode.
/// </summary>
public enum DataGridNavigationMode
{
    /// <summary>
    /// The default navigation mode.
    /// </summary>
    Default,

    /// <summary>
    /// Cell navigation mode, allows grid cells navigation by pressing the Keyboard's ArrowLeft, ArrowUp, ArrowRight, ArrowDown, Home, End, PageUp and PageDown keys.
    /// </summary>
    Cell,

    /// <summary>
    /// Row navigation mode, allows grid row navigation by pressing the Keyboard's ArrowUp, ArrowDown, Home, End, PageUp and PageDown keys.
    /// </summary>
    Row,
}