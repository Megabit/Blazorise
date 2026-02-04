#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// A multi select column for the <see cref="DataGrid{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The type of the model that the <see cref="DataGrid{TItem}"/> will handle.</typeparam>
public partial class DataGridMultiSelectColumn<TItem> : DataGridColumn<TItem>
{
    #region Constructors

    public DataGridMultiSelectColumn()
    {
        // Avoid row click side-effects when interacting with the header/body checkboxes.
        PreventRowClick = true;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override DataGridColumnType ColumnType => DataGridColumnType.MultiSelect;

    /// <summary>
    /// Template to customize multi select checkbox.
    /// </summary>
    [Parameter] public RenderFragment<MultiSelectContext<TItem>> MultiSelectTemplate { get; set; }

    /// <summary>
    /// Event that is being triggered when the row selection is changed.
    /// </summary>
    [Parameter] public EventCallback<DataGridMultiSelectionChangedEventArgs<TItem>> SelectionChanged { get; set; }

    #endregion
}