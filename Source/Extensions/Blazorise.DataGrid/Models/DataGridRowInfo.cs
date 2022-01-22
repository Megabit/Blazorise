#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.DataGrid.Models
{
    /// <summary>
    /// Holds the basic information about the datagrid row.
    /// </summary>
    public class DataGridRowInfo<TItem>
    {
        private bool hasDetailRow;

        /// <summary>
        /// Initializes a new instance of row info.
        /// </summary>
        /// <param name="item">Row Item</param>
        /// <param name="columns">Row Columns</param>
        public DataGridRowInfo( TItem item, IEnumerable<DataGridColumn<TItem>> columns )
        {
            Item = item;
            Columns = columns;
        }

        /// <summary>
        /// Gets the list of columns.
        /// </summary>
        public IEnumerable<DataGridColumn<TItem>> Columns { get; }

        /// <summary>
        /// Holds the Row's Item
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Whether Row should display DetailRow
        /// </summary>
        public bool HasDetailRow => hasDetailRow;

        /// <summary>
        /// Sets whether Row should display DetailRow
        /// </summary>
        /// <param name="hasDetailRow">DetailRow evaluation result.</param>
        /// <param name="toggleable">If true toggles the detail row.</param>
        public void SetRowDetail( bool hasDetailRow, bool toggleable )
            => this.hasDetailRow = (toggleable && !this.hasDetailRow & hasDetailRow) || (!toggleable && hasDetailRow );

        /// <summary>
        /// Toggles the DetailRow
        /// </summary>
        public void ToggleDetailRow()
            => hasDetailRow = !hasDetailRow;
    }
}
