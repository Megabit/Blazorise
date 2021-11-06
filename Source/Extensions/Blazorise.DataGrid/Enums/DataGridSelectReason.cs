using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.DataGrid.Enums
{
    /// <summary>
    /// Defines the Select Reason of the DataGrid Selection.
    /// </summary>
    public enum DataGridSelectReason
    {
        /// <summary>
        /// Row has been clicked.
        /// </summary>
        RowClick,
        /// <summary>
        /// Multi select has been triggered.
        /// </summary>
        MultiSelectClick,
        /// <summary>
        /// Multi select all has been triggered.
        /// </summary>
        MultiSelectAll
    }
}
