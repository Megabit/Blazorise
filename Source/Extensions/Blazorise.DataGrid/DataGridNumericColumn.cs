#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.DataGrid
{
    public class DataGridNumericColumn<TItem> : DataGridColumn<TItem>
    {
        public override DataGridColumnType ColumnType => DataGridColumnType.Numeric;
    }
}
