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
    /// Cell navigation mode, allows grid cells navigation by pressing the Keyboard's ArrowLeft, ArrowUp, ArrowRight and ArrowDown keys.
    /// </summary>
    Cell,
    /// <summary>
    /// Row navigation mode, allows grid row navigation by pressing the Keyboard's ArrowUp and ArrowDown keys.
    /// </summary>
    Row
}
