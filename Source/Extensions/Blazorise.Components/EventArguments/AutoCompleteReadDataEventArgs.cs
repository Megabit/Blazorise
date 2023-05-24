#region Using directives
using System.Threading;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Provides all the information for loading the <see cref="Autocomplete{TItem, TValue}"/> data manually.
/// </summary>
public class AutocompleteReadDataEventArgs
{
    /// <summary>
    /// Initializes a new instance of read-data event argument.
    /// </summary>
    /// <param name="searchValue">The value to search.</param>
    /// <param name="virtualizeOffset">Requested data start index by Virtualize.</param>
    /// <param name="virtualizeCount">Max number of items requested by Virtualize.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public AutocompleteReadDataEventArgs( string searchValue, int virtualizeOffset = 0,
        int virtualizeCount = 0, CancellationToken cancellationToken = default )
    {
        CancellationToken = cancellationToken;
        SearchValue = searchValue;
        VirtualizeOffset = virtualizeOffset;
        VirtualizeCount = virtualizeCount;
    }

    /// <summary>
    /// Gets the search value.
    /// </summary>
    public string SearchValue { get; private set; }

    /// <summary>
    /// Gets the requested data start index by Virtualize.
    /// </summary>
    public int VirtualizeOffset { get; }

    /// <summary>
    /// Gets the max number of items requested by Virtualize.
    /// </summary>
    public int VirtualizeCount { get; }

    /// <summary>
    /// Gets the CancellationToken.
    /// </summary>
    public CancellationToken CancellationToken { get; private set; }
}