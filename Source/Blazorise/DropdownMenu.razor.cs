#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class DropdownMenu : BaseComponent
    {
        #region Members

        private bool visible;

        private bool rightAligned;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DropdownMenu() );
            builder.Append( ClassProvider.DropdownMenuVisible( Visible ) );
            builder.Append( ClassProvider.DropdownMenuRight(), RightAligned );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( ParentDropdown != null )
            {
                Visible = ParentDropdown.Visible;

                ParentDropdown.StateChanged += OnDropdownStateChanged;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentDropdown != null )
                {
                    ParentDropdown.StateChanged -= OnDropdownStateChanged;
                }
            }

            base.Dispose( disposing );
        }

        private void OnDropdownStateChanged( object sender, DropdownStateEventArgs e )
        {
            Visible = e.Visible;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Handles the visibility of dropdown menu.
        /// </summary>
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
        /// Right aligned dropdown menu.
        /// </summary>
        [Parameter]
        public bool RightAligned
        {
            get => rightAligned;
            set
            {
                rightAligned = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
