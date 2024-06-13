namespace Blazorise.DataGrid;

/// <summary>
/// Defines the options for <see cref="DataGrid{TItem}"/> edit mode.
/// </summary>
public class DataGridEditModeOptions
{

    /// <summary>
    /// When the DataGridEditMode is set to <see cref="DataGridEditMode.Cell" /> configures whether the cell enters edit mode on single click.
    /// Defaults to false.
    /// </summary>
    public bool CellEditOnSingleClick { get; set; }

    /// <summary>
    /// When the DataGridEditMode is set to <see cref="DataGridEditMode.Cell" /> configures whether the cell enters edit mode on double click.
    /// Defaults to true.
    /// </summary>
    public bool CellEditOnDoubleClick { get; set; } = true;

    /// <summary>
    /// When the DataGridEditMode is set to <see cref="DataGridEditMode.Cell" />  configures whether the text is selected when the cell enters edit mode.
    /// </summary>
    public bool CellEditSelectTextOnEdit { get; set; }
}