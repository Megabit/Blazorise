#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarMenu : BaseComponent
    {
        #region Members

        private BarState parentBarState;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarMenu( ParentBarState.Mode ) );
            builder.Append( ClassProvider.BarMenuShow( ParentBarState.Mode ), ParentBarState.Visible );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [CascadingParameter]
        protected BarState ParentBarState
        {
            get => parentBarState;
            set
            {
                if ( parentBarState == value )
                    return;

                parentBarState = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
