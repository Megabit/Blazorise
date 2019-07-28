#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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

    /// <summary>
    /// Represents the base class for cancellable events when datagrid item is saving.
    /// </summary>
    /// <typeparam name="TItem">Model type param.</typeparam>
    /// <typeparam name="TValues">Values type param.</typeparam>
    public class CancellableRowChange<TItem, TValues> : CancellableRowChange<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the cancelable event argument.
        /// </summary>
        /// <param name="item">Saved item.</param>
        /// <param name="values">Edited values.</param>
        public CancellableRowChange( TItem item, TValues values )
            : base( item )
        {
            Values = values;
        }

        /// <summary>
        /// Values that are being edited by the datagrid.
        /// </summary>
        public TValues Values { get; }
    }

    /// <summary>
    /// Represents the base class for items saved by the datagrid.
    /// </summary>
    /// <typeparam name="TItem">Model type param.</typeparam>
    /// <typeparam name="TValues">Values type param.</typeparam>
    public class SavedRowItem<TItem, TValues> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the event argument.
        /// </summary>
        /// <param name="item">Saved item.</param>
        /// <param name="values">Edited values.</param>
        public SavedRowItem( TItem item, TValues values )
        {
            Item = item;
            Values = values;
        }

        /// <summary>
        /// Gets the model that was saved.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Values that are being edited by the datagrid.
        /// </summary>
        public TValues Values { get; }
    }
}
