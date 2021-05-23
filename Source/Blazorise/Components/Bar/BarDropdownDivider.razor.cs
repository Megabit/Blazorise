#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Divider that can be placed between <see cref="BarDropdownItem"/>'s.
    /// </summary>
    public partial class BarDropdownDivider : BaseComponent
    {
        #region Members

        private BarState parentBarState;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownDivider( ParentBarState?.Mode ?? BarMode.Horizontal ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Cascaded <see cref="Bar"/> component state object.
        /// </summary>
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

        #endregion
    }
}
