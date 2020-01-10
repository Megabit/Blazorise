#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Helper extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the next available direction based on the current one.
        /// </summary>
        /// <param name="direction">Current sort direction.</param>
        /// <returns>Returns the next available sort direction.</returns>
        public static SortDirection NextDirection( this SortDirection direction )
        {
            switch ( direction )
            {
                case SortDirection.None:
                    return SortDirection.Ascending;
                case SortDirection.Ascending:
                    return SortDirection.Descending;
                default:
                    return SortDirection.None;
            }
        }
    }
}
