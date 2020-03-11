#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        private event FilterTrigger Filter;

        public void Subscribe(FilterTrigger listener)
        {
            Filter += listener;
        }

        public void TriggerFilterEvent(string value )
        {
            Filter?.Invoke(value );
        }

        public delegate void FilterTrigger( string value );
    }
}
