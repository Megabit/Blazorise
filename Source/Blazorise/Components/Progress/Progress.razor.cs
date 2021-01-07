#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Main component for stacked progress bars.
    /// </summary>
    public partial class Progress : BaseComponent
    {
        #region Members

        private Size size = Size.None;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Progress() );
            builder.Append( ClassProvider.ProgressSize( Size ), Size != Size.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Size of the progress bar.
        /// </summary>
        [Parameter]
        public Size Size
        {
            get => size;
            set
            {
                size = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Progress"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
