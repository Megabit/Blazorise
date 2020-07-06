#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Context for pagination button.
    /// </summary>
    public class PageButtonContext
    {
        /// <summary>
        /// Default context constructor.
        /// </summary>
        /// <param name="pageNumer">Button page number .</param>
        /// <param name="active">Indicates if page is active.</param>
        public PageButtonContext( int pageNumer, bool active )
        {
            PageNumer = pageNumer;
            Active = active;
        }

        /// <summary>
        /// Gets the page number.
        /// </summary>
        public int PageNumer { get; }

        /// <summary>
        /// Get the flag that indicates if the page is active.
        /// </summary>
        public bool Active { get; }
    }
}
