#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
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
