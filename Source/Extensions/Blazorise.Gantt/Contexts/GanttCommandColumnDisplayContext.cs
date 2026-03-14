#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Context passed to <see cref="GanttCommandColumn{TItem}.DisplayTemplate"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class GanttCommandColumnDisplayContext<TItem> : GanttColumnDisplayContext<TItem>
{
    /// <summary>
    /// Creates a new <see cref="GanttCommandColumnDisplayContext{TItem}"/>.
    /// </summary>
    public GanttCommandColumnDisplayContext( Gantt<TItem> gantt, BaseGanttColumn<TItem> column, TItem item, string key, object value, string text, int level, bool hasChildren, bool expanded, bool selected, bool focused, Func<Task> toggleNode, double treeToggleWidth, bool canAddChild, Func<Task> addChildCommand, bool canEdit, Func<Task> editCommand, bool canDelete, Func<Task> deleteCommand )
        : base( gantt, column, item, key, value, text, level, hasChildren, expanded, selected, focused, toggleNode, treeToggleWidth )
    {
        CanAddChild = canAddChild;
        AddChildCommand = addChildCommand ?? ( () => Task.CompletedTask );
        CanEdit = canEdit;
        EditCommand = editCommand ?? ( () => Task.CompletedTask );
        CanDelete = canDelete;
        DeleteCommand = deleteCommand ?? ( () => Task.CompletedTask );
    }

    /// <summary>
    /// True when add-child action is available.
    /// </summary>
    public bool CanAddChild { get; }

    /// <summary>
    /// Callback used to add a child task.
    /// </summary>
    public Func<Task> AddChildCommand { get; }

    /// <summary>
    /// True when edit action is available.
    /// </summary>
    public bool CanEdit { get; }

    /// <summary>
    /// Callback used to edit current task.
    /// </summary>
    public Func<Task> EditCommand { get; }

    /// <summary>
    /// True when delete action is available.
    /// </summary>
    public bool CanDelete { get; }

    /// <summary>
    /// Callback used to delete current task.
    /// </summary>
    public Func<Task> DeleteCommand { get; }

    /// <summary>
    /// Adds child task for current row.
    /// </summary>
    public Task AddChild()
        => AddChildCommand.Invoke();

    /// <summary>
    /// Opens edit for current row.
    /// </summary>
    public Task Edit()
        => EditCommand.Invoke();

    /// <summary>
    /// Deletes current row.
    /// </summary>
    public Task Delete()
        => DeleteCommand.Invoke();
}