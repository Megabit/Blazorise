#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The near part of the menu, which appears next to the navbar brand on desktop.
    /// </summary>
    public partial class BarStart : BaseComponent
    {
        #region Members

        private BarState parentBarState;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarStart( ParentBarState?.Mode ?? BarMode.Horizontal ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="BarStart"/> component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

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
