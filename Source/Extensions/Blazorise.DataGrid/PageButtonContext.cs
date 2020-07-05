using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorise.DataGrid
{
    public class PageButtonContext
    {
        public PageButtonContext(int pageNumer, bool isActive)
        {
            PageNumer = pageNumer;
            IsActive = isActive;
        }

        public int PageNumer { get; }
        public bool IsActive { get; }
    }
}
