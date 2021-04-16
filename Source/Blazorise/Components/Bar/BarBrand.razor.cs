#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Part of the <see cref="Bar"/> component that is always visible, and which usually contains
    /// the logo and optionally some links or icons.
    /// </summary>
    public partial class BarBrand : BaseComponent
    {
        #region Members

        private BarState parentBarState;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarBrand( ParentBarState?.Mode ?? BarMode.Horizontal ) );

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

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="BarDropdownItem"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
