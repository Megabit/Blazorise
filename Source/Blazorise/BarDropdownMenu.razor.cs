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
    public partial class BarDropdownMenu : BaseComponent
    {
        #region Members

        private bool rightAligned;

        private BarDropdownStore parentStore;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownMenu( ParentStore.Mode ) );
            builder.Append( ClassProvider.BarDropdownMenuVisible( ParentStore.Mode, ParentStore.Visible && ParentStore.Mode != BarMode.VerticalSmall ) );
            builder.Append( ClassProvider.BarDropdownMenuRight( ParentStore.Mode ), RightAligned );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

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

        [CascadingParameter]
        protected BarDropdownStore ParentStore
        {
            get => parentStore;
            set
            {
                if ( parentStore == value )
                    return;

                parentStore = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
