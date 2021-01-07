#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Context for editors in datagrid cell.
    /// </summary>
    public class CellEditContext
    {
        /// <summary>
        /// Gets or sets the editor value.
        /// </summary>
        public object CellValue { get; set; }
    }

    /// <summary>
    /// Abstraction of <see cref="CellEditContext"/> that holds the reference to the model being edited.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class CellEditContext<TItem> : CellEditContext
    {
        public CellEditContext( TItem item )
        {
            Item = item;
        }

        /// <summary>
        /// Gets the reference to the model that is currently in edit mode.
        /// </summary>
        /// <remarks>
        /// Note that this model is used only for reading and you should never update it directly or any
        /// of it's field members. For writing the edited value you must use <see cref="CellEditContext.CellValue"/>.
        /// </remarks>
        public TItem Item { get; }
    }
}
