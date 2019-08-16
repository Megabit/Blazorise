#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Sidebar
{
    public class SidebarData
    {
        public string BrandAddress { get; set; }

        public List<SidebarNode> Items { get; set; }
    }

    public class SidebarNode
    {
        public string Tooltip { get; set; }

        public string Text { get; set; }

        public string To { get; set; }

        public List<SidebarNode> Items { get; set; }
    }
}
