#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar
{
    public partial class SidebarItem : BaseComponent
    {
        #region Members

        private bool hasLink;

        private bool hasSubItem;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "sidebar-item" );

            base.BuildClasses( builder );
        }

        internal void NotifyHasSidebarLink()
        {
            hasLink = true;
        }

        internal void NotifyHasSidebarSubItem()
        {
            hasSubItem = true;
        }

        #endregion

        #region Properties

        public bool HasLink => hasLink;

        public bool HasSubItem => hasSubItem;

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
