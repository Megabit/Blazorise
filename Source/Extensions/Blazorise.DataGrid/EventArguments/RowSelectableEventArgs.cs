#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Provides all the information about the RowSelectable event.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class RowSelectableEventArgs<TItem> : EventArgs
    {
        /// <summary>
        /// Default constructors.
        /// </summary>
        /// <param name="item">Model that belongs to the grid row.</param>
        /// <param name="selectReason">Indicates the Row Select Reason</param>
        public RowSelectableEventArgs( TItem item, DataGridSelectReason selectReason)
        {
            Item = item;
            SelectReason = selectReason;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; }
     
        /// <summary>
        /// Gets the Row Select Reason
        /// </summary>
        public DataGridSelectReason SelectReason { get; }
    }
}