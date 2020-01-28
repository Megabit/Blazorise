#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarDropdownMenu : BaseComponent
    {
        #region Members

        private bool visible;

        private bool isRightAligned;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownMenu() );
            builder.Append( ClassProvider.BarDropdownMenuShow(), Visible );
            builder.Append( ClassProvider.BarDropdownMenuRight(), IsRightAligned );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( ParentBarDropdown != null )
            {
                Visible = ParentBarDropdown.Visible;

                ParentBarDropdown.StateChanged += OnBarDropdownStateChanged;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing)
            {
                if ( ParentBarDropdown != null )
                {
                    ParentBarDropdown.StateChanged -= OnBarDropdownStateChanged;
                }
            }

            base.Dispose( disposing );
        }

        private void OnBarDropdownStateChanged( object sender, BarDropdownStateEventArgs e )
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
        public bool IsRightAligned
        {
            get => isRightAligned;
            set
            {
                isRightAligned = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
