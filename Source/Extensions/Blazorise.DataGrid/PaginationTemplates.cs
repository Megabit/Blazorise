using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace Blazorise.DataGrid
{
    public class PaginationTemplates
    {
        public RenderFragment FirstPageButtonTemplate { get; set; }
        public RenderFragment LastPageButtonTemplate { get; set; }
        public RenderFragment PreviousPageButtonTemplate { get; set; }
        public RenderFragment NextPageButtonTemplate { get; set; }
        public RenderFragment ItemsPerPageTemplate { get; set; }
        public RenderFragment<PaginationContext> TotalItemsShortTemplate { get; set; }
        public RenderFragment<PaginationContext> TotalItemsTemplate { get; set; }
    }
}
