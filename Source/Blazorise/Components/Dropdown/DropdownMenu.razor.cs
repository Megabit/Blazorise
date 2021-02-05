#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class DropdownMenu : BaseComponent
    {
        #region Members

        private DropdownState parentDropdownState;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentDropdown != null )
            {
                ParentDropdown.VisibleChanged += OnVisibleChanged;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentDropdown != null )
                {
                    ParentDropdown.VisibleChanged -= OnVisibleChanged;
                }
            }

            base.Dispose( disposing );
        }

        protected virtual void OnVisibleChanged( object sender, bool e )
        {
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DropdownMenu() );
            builder.Append( ClassProvider.DropdownMenuVisible( ParentDropdownState.Visible ) );
            builder.Append( ClassProvider.DropdownMenuRight(), ParentDropdownState.RightAligned );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [CascadingParameter]
        protected DropdownState ParentDropdownState
        {
            get => parentDropdownState;
            set
            {
                if ( parentDropdownState == value )
                    return;

                parentDropdownState = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
