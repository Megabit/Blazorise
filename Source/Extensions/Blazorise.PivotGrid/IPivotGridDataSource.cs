#region Using directives
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Represents an external data provider for the <see cref="PivotGrid{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public interface IPivotGridDataSource<TItem>
{
    /// <summary>
    /// Reads pivot grid data for the supplied request.
    /// </summary>
    /// <param name="request">Current pivot grid data request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Data result used by the pivot grid.</returns>
    Task<PivotGridDataResult<TItem>> ReadDataAsync( PivotGridDataRequest request, CancellationToken cancellationToken );
}