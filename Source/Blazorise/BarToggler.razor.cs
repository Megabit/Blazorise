#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarToggler : BaseComponent
    {
        #region Members

        private bool visible;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarToggler() );
            builder.Append( ClassProvider.BarTogglerCollapsed( Visible ) );

            base.BuildClasses( builder );
        }

        protected void ClickHandler()
        {
            // NOTE: is this right?
            if ( Clicked == null )
                ParentBar?.Toggle();
            else
                Clicked?.Invoke();
        }

        protected override void OnInitialized()
        {
            if ( ParentBar != null )
            {
                Visible = ParentBar.Visible;

                ParentBar.StateChanged += OnBarStateChanged;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentBar != null )
                {
                    ParentBar.StateChanged -= OnBarStateChanged;
                }
            }

            base.Dispose( disposing );
        }

        private void OnBarStateChanged( object sender, BarStateEventArgs e )
        {
            Visible = e.Visible;
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

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        [CascadingParameter] protected Bar ParentBar { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
