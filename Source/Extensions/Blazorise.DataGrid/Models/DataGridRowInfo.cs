using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.DataGrid.Models
{

    public class DataGridRowInfo<TItem>
    {
        private bool showDetail;

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

        public IEnumerable<DataGridColumn<TItem>> Columns {get;}

        public TItem Item { get; }

        public bool ShowDetail => showDetail;

        public void SetRowDetail( bool rowDetail )
            => showDetail = !showDetail & rowDetail;

        public void ToggleRowDetail(  )
            => showDetail = !showDetail;
    }
}
