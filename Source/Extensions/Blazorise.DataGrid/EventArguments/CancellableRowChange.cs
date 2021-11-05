#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Represents the base class for cancellable events when datagrid item is saving.
    /// </summary>
    /// <typeparam name="TItem">Model type param.</typeparam>
    public class CancellableRowChange<TItem> : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the cancelable event argument.
        /// </summary>
        /// <param name="item">Saved item.</param>
        public CancellableRowChange( TItem item )
        {
            Item = item;
        }

        /// <summary>
        /// Gets the model that was saved.
        /// </summary>
        public TItem Item { get; }
    }
}