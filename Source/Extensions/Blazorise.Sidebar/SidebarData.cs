#region Using directives
using System.Collections.Generic;
using System.Runtime.Serialization;
#endregion

namespace Blazorise.Sidebar
{
    /// <summary>
    /// Holds the information to generate the sidebar items.
    /// </summary>
    [DataContract]
    public class SidebarInfo
    {
        /// <summary>
        /// Sidebar brand located in sidebar header.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public SidebarBrandInfo Brand { get; set; }

        /// <summary>
        /// Collection of first-level sidebar items.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public List<SidebarItemInfo> Items { get; set; }
    }

    /// <summary>
    /// Information to genarate the sidebar brand.
    /// </summary>
    public class SidebarBrandInfo
    {
        /// <summary>
        /// Brand text.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public string Text { get; set; }

        /// <summary>
        /// Brand url.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public string To { get; set; }
    }

    /// <summary>
    /// Information to genarate the sidebar item.
    /// </summary>
    public class SidebarItemInfo
    {
        /// <summary>
        /// Tooltip for the sidebar link.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public string Tooltip { get; set; }

        /// <summary>
        /// Text for the sidebar link.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public string Text { get; set; }

        /// <summary>
        /// Url for the sidebar link.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public string To { get; set; }

        /// <summary>
        /// Icon for the sidebar link.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public object Icon { get; set; }

        /// <summary>
        /// Whether the item and all sub-items are shown.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public bool Visible { get; set; }

        /// <summary>
        /// Collection of item sub-items.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public List<SidebarItemInfo> SubItems { get; set; }

        /// <summary>
        /// This is needed only so that reference across rendering states is saved.
        /// </summary>
        [IgnoreDataMember]
        internal SidebarSubItem SubItemReference { get; set; }
    }
}
