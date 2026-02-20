namespace Blazorise.DataGrid;

/// <summary>
/// Defines how row expansion can be triggered.
/// </summary>
public enum DataGridExpandTrigger
{
    /// <summary>
    /// Row expansion is toggled by clicking the toggle icon.
    /// </summary>
    ToggleClick,

    /// <summary>
    /// Row expansion is toggled by clicking the row.
    /// </summary>
    RowClick,

    /// <summary>
    /// Row expansion is toggled by clicking either the row or the toggle icon.
    /// </summary>
    RowAndToggleClick
}