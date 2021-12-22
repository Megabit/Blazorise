using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.PivotGrid
{
    public partial class PivotGrid<TItem> : ComponentBase
    {
        
        [Parameter]
        public IEnumerable<TItem> Data { get; set; }


        protected List<PivotGridValue<TItem>> Values { get; set; } = new();
        protected List<PivotGridColumn<TItem>> Columns { get; set; } = new();
        protected List<PivotGridRow<TItem>> Rows { get; set; } = new();


        [Parameter]
        public RenderFragment PivotGridColumns { get; set; }

        [Parameter]
        public RenderFragment PivotGridRows { get; set; }

        [Parameter]
        public RenderFragment PivotGridValues { get; set; }



        public void AddColumn( PivotGridColumn<TItem> column )
        {
            Columns.Add( column );
        }

        public bool RemoveColumn( PivotGridColumn<TItem> column )
        {
            return Columns.Remove( column );
        }

        public void AddRow( PivotGridRow<TItem> row )
        {
            Rows.Add( row );
        }

        public bool RemoveRow( PivotGridRow<TItem> row )
        {
            return Rows.Remove( row );
        }

        public void AddValue( PivotGridValue<TItem> value )
        {
            Values.Add( value );
        }

        public bool RemoveValue( PivotGridValue<TItem> value )
        {
            return Values.Remove( value );
        }
    }
}
