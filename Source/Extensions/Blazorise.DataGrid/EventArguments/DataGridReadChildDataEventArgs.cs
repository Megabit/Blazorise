#region Using directives
using System;
using System.Collections.Generic;
using System.Threading;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides all the information for loading datagrid hierarchy child data.
/// </summary>
/// <typeparam name="TItem">Type of the data model.</typeparam>
public class DataGridReadChildDataEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of read-child-data event argument.
    /// </summary>
    /// <param name="item">Parent row item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public DataGridReadChildDataEventArgs( TItem item, CancellationToken cancellationToken = default )
    {
        Item = item;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Gets the parent row item for which children are requested.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets or sets the resolved child rows.
    /// </summary>
    public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; }
}