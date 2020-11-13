#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public class PaginationTemplates<TItem>
    {
        public RenderFragment FirstPageButtonTemplate { get; set; }

        public RenderFragment LastPageButtonTemplate { get; set; }

        public RenderFragment PreviousPageButtonTemplate { get; set; }

        public RenderFragment NextPageButtonTemplate { get; set; }

        public RenderFragment ItemsPerPageTemplate { get; set; }

        public RenderFragment<PaginationContext<TItem>> TotalItemsShortTemplate { get; set; }

        public RenderFragment<PaginationContext<TItem>> TotalItemsTemplate { get; set; }
    }
}
