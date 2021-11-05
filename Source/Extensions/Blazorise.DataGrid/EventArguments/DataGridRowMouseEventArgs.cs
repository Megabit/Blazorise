#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Provides all the information about the clicked event on datagrid row.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class DataGridRowMouseEventArgs<TItem> : EventArgs
    {
        public DataGridRowMouseEventArgs( TItem item, BLMouseEventArgs mouseEventArgs )
        {
            Item = item;
            MouseEventArgs = mouseEventArgs;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Gets the mouse event details.
        /// </summary>
        public BLMouseEventArgs MouseEventArgs { get; }
    }
}