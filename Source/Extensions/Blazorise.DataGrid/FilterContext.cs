#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Context for editors in datagrid filter section.
    /// </summary>
    public class FilterContext
    {
        /// <summary>
        /// Gets or sets the filter value.
        /// </summary>
        public string SearchValue { get; set; }
    }
}
