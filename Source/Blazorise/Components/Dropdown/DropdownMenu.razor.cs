#region Using directives
using Blazorise.Stores;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class DropdownMenu : BaseComponent
    {
        #region Members

        private DropdownStore parentDropdownStore;

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
            builder.Append( ClassProvider.DropdownMenuVisible( ParentDropdownStore.Visible ) );
            builder.Append( ClassProvider.DropdownMenuRight(), ParentDropdownStore.RightAligned );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [CascadingParameter]
        protected DropdownStore ParentDropdownStore
        {
            get => parentDropdownStore;
            set
            {
                if ( parentDropdownStore == value )
                    return;

                parentDropdownStore = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
