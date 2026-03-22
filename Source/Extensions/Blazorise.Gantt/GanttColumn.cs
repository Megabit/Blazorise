#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Defines a regular tree column in <see cref="Gantt{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class GanttColumn<TItem> : BaseGanttColumn<TItem>
{
    #region Methods

    /// <summary>
    /// Gets whether the column value is editable for current edit state.
    /// </summary>
    /// <param name="editState">Current edit state.</param>
    /// <returns>True when editable; otherwise false.</returns>
    public bool CellValueIsEditable( GanttEditState editState )
    {
        if ( !Editable )
            return false;

        return editState switch
        {
            GanttEditState.New when CellsEditableOnNewCommand => true,
            GanttEditState.Edit when CellsEditableOnEditCommand => true,
            _ => false,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets whether this column is editable in edit forms.
    /// </summary>
    [Parameter] public bool Editable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this column is editable during new-item command.
    /// </summary>
    [Parameter] public bool CellsEditableOnNewCommand { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this column is editable during edit-item command.
    /// </summary>
    [Parameter] public bool CellsEditableOnEditCommand { get; set; } = true;

    /// <summary>
    /// Gets or sets custom display template.
    /// </summary>
    [Parameter]
    public new RenderFragment<GanttColumnDisplayContext<TItem>> DisplayTemplate
    {
        get => base.DisplayTemplate;
        set => base.DisplayTemplate = value;
    }

    #endregion
}