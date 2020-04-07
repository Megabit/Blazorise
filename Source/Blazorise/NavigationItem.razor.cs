#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    [Obsolete( "This component has been deprecated. Use Bar component and it's sub-components instead." )]
    public partial class NavigationItem : BaseComponent
    {
        #region Members

        private bool active;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.NavItem() );
            builder.Append( ClassProvider.NavLink() );
            builder.Append( ClassProvider.Active(), Active );

            base.BuildClasses( builder );
        }

        protected void ClickHandler()
        {
            Clicked?.Invoke();
        }

        #endregion

        #region Properties

        [Parameter]
        public bool Active
        {
            get => active;
            set
            {
                active = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
