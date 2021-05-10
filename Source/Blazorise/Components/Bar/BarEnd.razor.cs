#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The far part of the menu, which appears at the end of the navbar.
    /// </summary>
    public partial class BarEnd : BaseComponent
    {
        #region Members

        private BarState parentBarState;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarEnd( ParentBarState?.Mode ?? BarMode.Horizontal ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="BarEnd"/> component.
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
