using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazorise.Components
{
    /// <summary>
    /// Provides all the information for loading the <see cref="Autocomplete{TItem, TValue}"/> data manually.
    /// </summary>
    public class AutoCompleteReadDataEventArgs
    {

        /// <summary>
        /// Initializes a new instance of read-data event argument.
        /// </summary>
        /// <param name="searchValue">The value to search.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public AutoCompleteReadDataEventArgs( string searchValue, CancellationToken cancellationToken = default )
        {
            CancellationToken = cancellationToken;
            SearchValue = searchValue;
        }

            public string SearchValue { get; private set; }


            /// <summary>
            /// Gets the CancellationToken
            /// </summary>
            public CancellationToken CancellationToken { get; private set; }
    }
}
