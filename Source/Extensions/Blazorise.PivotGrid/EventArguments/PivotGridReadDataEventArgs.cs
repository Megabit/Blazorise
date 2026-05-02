#region Using directives
using System;
using System.Collections.Generic;
using System.Threading;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Supplies data request information for the <see cref="PivotGrid{TItem}.ReadData"/> callback.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridReadDataEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridReadDataEventArgs{TItem}"/> class.
    /// </summary>
    /// <param name="request">Current data request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public PivotGridReadDataEventArgs( PivotGridDataRequest request, CancellationToken cancellationToken )
    {
        Request = request;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Gets the current data request.
    /// </summary>
    public PivotGridDataRequest Request { get; }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets or sets raw data items used by the pivot grid to build the result locally.
    /// </summary>
    public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Gets or sets total item count reported by the provider.
    /// </summary>
    public int? TotalItems { get; set; }

    /// <summary>
    /// Gets or sets whether returned data has already been paged by the callback.
    /// </summary>
    public bool IsPaged { get; set; }

    /// <summary>
    /// Gets or sets an already prepared pivot result.
    /// </summary>
    public PivotGridResult<TItem> Result { get; set; }
}