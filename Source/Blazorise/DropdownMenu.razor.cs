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

        private bool isOpen;

        private bool isRightAligned;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DropdownMenu() );
            builder.Append( ClassProvider.DropdownMenuShow(), IsOpen );
            builder.Append( ClassProvider.DropdownMenuRight(), IsRightAligned );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( ParentDropdown != null )
            {
                IsOpen = ParentDropdown.IsOpen;

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
            IsOpen = e.Opened;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Handles the visibility of dropdown menu.
        /// </summary>
        [Parameter]
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

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

        [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
