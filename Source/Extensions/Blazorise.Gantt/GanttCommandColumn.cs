#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Defines command column in <see cref="Gantt{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class GanttCommandColumn<TItem> : BaseGanttColumn<TItem>
{
    #region Constructors

    /// <summary>
    /// Creates a new <see cref="GanttCommandColumn{TItem}"/>.
    /// </summary>
    public GanttCommandColumn()
    {
        Sortable = false;
        Displayable = true;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override object GetValue( TItem item ) => null;

    /// <inheritdoc />
    public override object GetSortValue( TItem item ) => null;

    #endregion

    #region Properties

    /// <summary>
    /// Defines custom command display template.
    /// </summary>
    [Parameter] public new RenderFragment<GanttCommandColumnDisplayContext<TItem>> DisplayTemplate { get; set; }

    #endregion

}