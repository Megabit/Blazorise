using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorise.DataGrid
{
    public class PopupTitleContext<TItem>
    {
        public PopupTitleContext( TItem item, DataGridEditState editState )
        {
            Item = item;
            EditState = editState;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Gets the edit state of data grid.
        /// </summary>
        public DataGridEditState EditState { get; }
    }
}
