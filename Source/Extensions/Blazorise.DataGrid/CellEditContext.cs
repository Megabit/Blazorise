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

        /// <summary>
        /// Gets or sets the parent model of the edited CellValue - to be used in decision making
        /// TODO: Check if init would not be better than set
        /// </summary>
        public object Model { get; set; }
    }
}
