using System;
using System.Threading.Tasks;
using Blazorise;

namespace Blazorise.DataGrid;

/// <summary>
/// Context for customizing the hierarchy expand area in a row.
/// </summary>
/// <typeparam name="TItem">Type parameter for the model displayed in the <see cref="DataGrid{TItem}"/>.</typeparam>
public class DataGridExpandRowContext<TItem> : BaseTemplateContext<TItem>
{
    private readonly Func<Task> toggle;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridExpandRowContext{TItem}"/> class.
    /// </summary>
    /// <param name="item">The row item.</param>
    /// <param name="level">Hierarchy level where 0 is root.</param>
    /// <param name="expandable">Indicates whether the row can be expanded.</param>
    /// <param name="expanded">Indicates whether the row is currently expanded.</param>
    /// <param name="toggle">Callback to toggle the row expand state.</param>
    public DataGridExpandRowContext( TItem item, int level, bool expandable, bool expanded, Func<Task> toggle )
        : base( item )
    {
        Level = level;
        Expandable = expandable;
        Expanded = expanded;
        this.toggle = toggle;
    }

    /// <summary>
    /// Gets the hierarchy level where 0 is root.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Gets whether the row can be expanded.
    /// </summary>
    public bool Expandable { get; }

    /// <summary>
    /// Gets whether the row is currently expanded.
    /// </summary>
    public bool Expanded { get; }

    /// <summary>
    /// Toggles row expand state.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Toggle()
    {
        return toggle?.Invoke() ?? Task.CompletedTask;
    }
}