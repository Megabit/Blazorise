#region Using directives
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Holds the basic information about the datagrid row.
    /// </summary>
    public class DataGridRowInfo<TItem>
    {
        private bool showDetail;
        /// <summary>
        /// Initializes a new instance of row info.
        /// </summary>
        /// <param name="item">Row Item</param>
        public DataGridRowInfo( TItem item )
        {
            Item = item;
        }

        public TItem Item { get; }

        public bool ShowDetail => showDetail;

        public void SetShowDetail(bool toShowDetail)
            => showDetail = !showDetail & toShowDetail;

    }
}
