#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Context passed to <see cref="BaseGanttColumn{TItem}.HeaderTemplate"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class GanttColumnHeaderContext<TItem>
{
    /// <summary>
    /// Creates a new <see cref="GanttColumnHeaderContext{TItem}"/>.
    /// </summary>
    public GanttColumnHeaderContext( Gantt<TItem> gantt, BaseGanttColumn<TItem> column, string text, bool sortable, bool showSortIcon, SortDirection sortDirection, bool isCommandColumn, bool canAddTask, Func<Task> addTaskCommand, string addTaskText )
    {
        Gantt = gantt;
        Column = column;
        Text = text;
        Sortable = sortable;
        ShowSortIcon = showSortIcon;
        SortDirection = sortDirection;
        IsCommandColumn = isCommandColumn;
        CanAddTask = canAddTask;
        AddTaskCommand = addTaskCommand ?? ( () => Task.CompletedTask );
        AddTaskText = addTaskText;
    }

    /// <summary>
    /// Parent gantt.
    /// </summary>
    public Gantt<TItem> Gantt { get; }

    /// <summary>
    /// Column.
    /// </summary>
    public BaseGanttColumn<TItem> Column { get; }

    /// <summary>
    /// Header text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// True when column is sortable.
    /// </summary>
    public bool Sortable { get; }

    /// <summary>
    /// True when sort icon should be visible.
    /// </summary>
    public bool ShowSortIcon { get; }

    /// <summary>
    /// Current sort direction.
    /// </summary>
    public SortDirection SortDirection { get; }

    /// <summary>
    /// True when template is rendered for command column.
    /// </summary>
    public bool IsCommandColumn { get; }

    /// <summary>
    /// True when add-task action is available.
    /// </summary>
    public bool CanAddTask { get; }

    /// <summary>
    /// Callback used to create a new task.
    /// </summary>
    public Func<Task> AddTaskCommand { get; }

    /// <summary>
    /// Localized add-task label.
    /// </summary>
    public string AddTaskText { get; }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    public Task AddTask()
        => AddTaskCommand.Invoke();
}
