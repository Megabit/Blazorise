#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides all the information about a DataGrid grouping change.
/// </summary>
/// <typeparam name="TItem">Type of the data model.</typeparam>
public sealed class DataGridGroupingChangedEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of grouping-changed event argument.
    /// </summary>
    /// <param name="groupedColumns">Current grouped columns.</param>
    /// <param name="previousGroupedColumns">Previous grouped columns.</param>
    /// <param name="changeType">Type of grouping change.</param>
    /// <param name="addedColumn">Added grouped column, if any.</param>
    /// <param name="removedColumn">Removed grouped column, if any.</param>
    public DataGridGroupingChangedEventArgs(
        IReadOnlyList<DataGridColumn<TItem>> groupedColumns,
        IReadOnlyList<DataGridColumn<TItem>> previousGroupedColumns,
        DataGridGroupingChangeType changeType,
        DataGridColumn<TItem> addedColumn = null,
        DataGridColumn<TItem> removedColumn = null )
    {
        GroupedColumns = groupedColumns;
        PreviousGroupedColumns = previousGroupedColumns;
        ChangeType = changeType;
        AddedColumn = addedColumn;
        RemovedColumn = removedColumn;
    }

    /// <summary>
    /// Gets the current grouped columns.
    /// </summary>
    public IReadOnlyList<DataGridColumn<TItem>> GroupedColumns { get; }

    /// <summary>
    /// Gets the previous grouped columns.
    /// </summary>
    public IReadOnlyList<DataGridColumn<TItem>> PreviousGroupedColumns { get; }

    /// <summary>
    /// Gets the type of grouping change.
    /// </summary>
    public DataGridGroupingChangeType ChangeType { get; }

    /// <summary>
    /// Gets the added grouped column when <see cref="ChangeType"/> is <see cref="DataGridGroupingChangeType.Added"/>.
    /// </summary>
    public DataGridColumn<TItem> AddedColumn { get; }

    /// <summary>
    /// Gets the removed grouped column when <see cref="ChangeType"/> is <see cref="DataGridGroupingChangeType.Removed"/>.
    /// </summary>
    public DataGridColumn<TItem> RemovedColumn { get; }
}