#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Provides all the information about the current column sorting.
    /// </summary>
    public class DataGridSortChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructors.
        /// </summary>
        /// <param name="fieldName">Column field name.</param>
        /// <param name="sortDirection">Column sort direction.</param>
        public DataGridSortChangedEventArgs( string fieldName, SortDirection sortDirection )
        {
            FieldName = fieldName;
            SortDirection = sortDirection;
        }

        /// <summary>
        /// Gets the field name of the column that is being sorted.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// Gets the new sort direction of the specified field name.
        /// </summary>
        public SortDirection SortDirection { get; }
    }
}