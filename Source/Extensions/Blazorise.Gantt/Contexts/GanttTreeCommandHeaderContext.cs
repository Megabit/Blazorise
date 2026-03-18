using System;
using System.Threading.Tasks;

namespace Blazorise.Gantt;

/// <summary>
/// Context for rendering tree command header content.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttTreeCommandHeaderContext<TItem>
{
    /// <summary>
    /// Creates a new context.
    /// </summary>
    /// <param name="gantt">Parent Gantt component.</param>
    /// <param name="showAddTaskButton">Indicates whether add-task action is available.</param>
    /// <param name="addTask">Callback used to create a new task.</param>
    /// <param name="addTaskText">Localized add-task label.</param>
    public GanttTreeCommandHeaderContext( Gantt<TItem> gantt, bool showAddTaskButton, Func<Task> addTask, string addTaskText )
    {
        Gantt = gantt;
        ShowAddTaskButton = showAddTaskButton;
        AddTask = addTask;
        AddTaskText = addTaskText;
    }

    /// <summary>
    /// Gets the parent Gantt component.
    /// </summary>
    public Gantt<TItem> Gantt { get; }

    /// <summary>
    /// Gets whether add-task action is available.
    /// </summary>
    public bool ShowAddTaskButton { get; }

    /// <summary>
    /// Gets callback used to create a new task.
    /// </summary>
    public Func<Task> AddTask { get; }

    /// <summary>
    /// Gets localized add-task label.
    /// </summary>
    public string AddTaskText { get; }
}