#region Using directives
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Holds the basic information about the datagrid column.
    /// </summary>
    public class DataGridColumnInfo
    {
        /// <summary>
        /// Initializes a new instance of column info.
        /// </summary>
        /// <param name="field">Field name.</param>
        /// <param name="searchValue">Current search value.</param>
        /// <param name="direction">Current sort direction.</param>
        /// <param name="columnType">Current column type.</param>
        public DataGridColumnInfo( string field, string searchValue, SortDirection direction, DataGridColumnType columnType )
        {
            Field = field;
            SearchValue = searchValue;
            Direction = direction;
            ColumnType = columnType;
        }

        /// <summary>
        /// Gets the column or datasource field name.
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Gets the column search value.
        /// </summary>
        public string SearchValue { get; }

        /// <summary>
        /// Gets the column sort direction.
        /// </summary>
        public SortDirection Direction { get; }

        /// <summary>
        /// Gets the column type.
        /// </summary>
        public DataGridColumnType ColumnType { get; }
    }
}
