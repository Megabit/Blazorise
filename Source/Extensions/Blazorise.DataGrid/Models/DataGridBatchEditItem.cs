﻿#region Using directives
using System.Collections.Generic;
using Blazorise.Extensions;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Tracks the state of the batch edit item.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridBatchEditItem<TItem>
{
    /// <summary>
    /// Gets the model with the values before being edited.
    /// </summary>
    public TItem OldItem { get; }

    /// <summary>
    /// Gets the model with the edited values.
    /// </summary>
    public TItem NewItem { get; private set; }

    /// <summary>
    /// Values that were edited.
    /// </summary>
    public Dictionary<string, CellEditContext> Values { get; private set; }

    /// <summary>
    /// The edit state for this batch edit item.
    /// </summary>
    public DataGridBatchEditItemState State { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Batch Edit Item.
    /// </summary>
    /// <param name="oldItem">Old Saved item.</param>
    /// <param name="newItem">New Saved item.</param>
    /// <param name="editItemState">The edit state for this batch item.</param>
    /// <param name="values">Edited values.</param>
    public DataGridBatchEditItem( TItem oldItem, TItem newItem, DataGridBatchEditItemState editItemState, Dictionary<string, CellEditContext> values = null )
    {
        OldItem = oldItem;
        NewItem = newItem;
        Values = values;
        State = editItemState;
    }

    /// <summary>
    /// Updates an existing Edit Item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="values"></param>
    internal void UpdateEditItem( TItem item, Dictionary<string, CellEditContext> values )
    {
        NewItem = item;

        if ( State == DataGridBatchEditItemState.New )
        {
            Values = values;
            return;
        }


        if ( !Values.IsNullOrEmpty() )
        {
            if ( values.IsNullOrEmpty() )
                return;

            foreach ( var kvp in values )
            {
                if ( Values.ContainsKey( kvp.Key ) )
                {
                    if ( kvp.Value.Modified )
                    {
                        Values[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    Values.Add( kvp.Key, kvp.Value );
                }
            }
        }
        else
        {
            Values = values;
        }
    }

    /// <summary>
    /// Deletes an existing Edit Item
    /// </summary>
    internal void DeleteEditItem()
    {
        State = DataGridBatchEditItemState.Delete;
        NewItem = OldItem;
        Values = null;
    }

    /// <summary>
    /// Whether the corresponding field was modified.
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public bool IsModified( string field )
        => ( !string.IsNullOrWhiteSpace( field ) && Values.TryGetValue( field, out var context ) && context.Modified );
}

