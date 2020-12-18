#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Inner component of <see cref="Progress"/> component used to indicate the progress so far.
    /// </summary>
    public partial class ProgressBar : BaseComponent
    {
        #region Members

        private Background background = Background.None;

        private bool striped;

        private bool animated;

        private int? @value;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ProgressBar() );
            builder.Append( ClassProvider.ProgressBarWidth( Value ?? 0 ) );
            builder.Append( ClassProvider.ProgressBarColor( Background ), Background != Background.None );
            builder.Append( ClassProvider.ProgressBarStriped(), Striped );
            builder.Append( ClassProvider.ProgressBarAnimated(), Animated );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void BuildStyles( StyleBuilder builder )
        {
            if ( Value != null )
                builder.Append( StyleProvider.ProgressBarValue( Value ?? 0 ) );

            builder.Append( StyleProvider.ProgressBarSize( ParentProgress?.Size ?? Size.None ) );

            base.BuildStyles( builder );
        }

        /// <summary>
        /// Sets the progress bar <see cref="Animated"/> flag.
        /// </summary>
        /// <param name="animated">True to animate the progress bar.</param>
        public void Animate( bool animated )
        {
            Animated = animated;

            InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the progress bar background color.
        /// </summary>
        [Parameter]
        public Background Background
        {
            get => background;
            set
            {
                background = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Set to true to make the progress bar stripped.
        /// </summary>
        [Parameter]
        public bool Striped
        {
            get => striped;
            set
            {
                striped = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Set to true to make the progress bar animated.
        /// </summary>
        [Parameter]
        public bool Animated
        {
            get => animated;
            set
            {
                animated = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Minimum value of the progress bar.
        /// </summary>
        [Parameter] public int Min { get; set; } = 0;

        /// <summary>
        /// Maximum value of the progress bar.
        /// </summary>
        [Parameter] public int Max { get; set; } = 100;

        /// <summary>
        /// The progress value.
        /// </summary>
        [Parameter]
        public int? Value
        {
            get => @value;
            set
            {
                if ( this.@value == value )
                    return;

                this.@value = value;

                DirtyClasses();
                DirtyStyles();
            }
        }

        [CascadingParameter] protected Progress ParentProgress { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="ProgressBar"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
