#region Using directives
using System.Threading;
#endregion

namespace Blazorise.Components
{
    /// <summary>
    /// Provides all the information for loading the <see cref="Autocomplete{TItem, TValue}"/> data manually.
    /// </summary>
    public class AutocompleteReadDataEventArgs
    {
        /// <summary>
        /// Initializes a new instance of read-data event argument.
        /// </summary>
        /// <param name="searchValue">The value to search.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public AutocompleteReadDataEventArgs( string searchValue, CancellationToken cancellationToken = default )
        {
            CancellationToken = cancellationToken;
            SearchValue = searchValue;
        }

        /// <summary>
        /// Gets the search value.
        /// </summary>
        public string SearchValue { get; private set; }

        /// <summary>
        /// Gets the CancellationToken.
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }
    }
}
