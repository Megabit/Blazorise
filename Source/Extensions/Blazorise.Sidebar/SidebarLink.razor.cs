#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar
{
    public abstract class BaseSidebarLink : BaseComponent
    {
        #region Members

        private bool visible;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "sidebar-link" );
            builder.Append( "collapsed", !Visible );

            base.BuildClasses( builder );
        }

        protected void ClickHandler()
        {
            Click?.Invoke();

            if ( To == null )
            {
                Visible = !Visible;

                StateHasChanged();

                Toggled?.Invoke( Visible );
            }
        }

        #endregion

        #region Properties

        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;

                DirtyClasses();
            }
        }

        [Parameter] public string To { get; set; }

        [Parameter] public Match Match { get; set; } = Match.All;

        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Click { get; set; }

        [Parameter] public Action<bool> Toggled { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
