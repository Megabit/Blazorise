using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.PivotGrid
{
    public partial class PivotGridColumn<TItem> : ComponentBase
    {

        [CascadingParameter]
        public PivotGrid<TItem> ParentPivotGrid { get; set; }

        [Parameter]
        public string Field { get; set; }

        [Parameter]
        public string Caption { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ParentPivotGrid?.AddColumn( this );
        }
    }
}
