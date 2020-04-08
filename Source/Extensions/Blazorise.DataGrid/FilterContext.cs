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

        private event FilterChangedEventHandler FilterChanged;

        public void Subscribe( FilterChangedEventHandler listener )
        {
            FilterChanged += listener;
        }

        public void Unsubscribe( FilterChangedEventHandler listener )
        {
            FilterChanged -= listener;
        }

        public void TriggerFilterChange( string value )
        {
            FilterChanged?.Invoke( value );
        }

        public delegate void FilterChangedEventHandler( string value );
    }
}
