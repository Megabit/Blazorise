﻿namespace Blazorise.DataGrid;

/// <summary>
/// A DataGrid column displaying state container.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridColumnDisplayingState<TItem>
{
    /// <summary>
    /// Initializes a new instance of column filter state.
    /// </summary>
    /// <param name="fieldName">Field name.</param>
    /// <param name="displaying">Current displaying state.</param>
    public DataGridColumnDisplayingState( string fieldName, bool displaying )
    {
        FieldName = fieldName;
        Displaying = displaying;
    }

    /// <summary>
    /// Gets the column or datasource field name.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the column displaying state.
    /// </summary>
    public bool Displaying { get; }
}