#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A responsive and flexible pagination component.
    /// </summary>
    public partial class Pagination : BaseComponent
    {
        #region Members

        private Size? size;

        private Alignment alignment = Alignment.None;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if ( Theme != null )
            {
                Theme.Changed += OnThemeChanged;
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                if ( Theme != null )
                {
                    Theme.Changed -= OnThemeChanged;
                }
            }

            return base.DisposeAsync( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Pagination() );
            builder.Append( ClassProvider.PaginationSize( ThemeSize ), ThemeSize != Blazorise.Size.None );
            builder.Append( ClassProvider.FlexAlignment( Alignment ), Alignment != Alignment.None );
            builder.Append( ClassProvider.BackgroundColor( Background ), Background != Background.None );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// An event raised when theme settings changes.
        /// </summary>
        /// <param name="sender">An object thet raised the event.</param>
        /// <param name="eventArgs"></param>
        private void OnThemeChanged( object sender, EventArgs eventArgs )
        {
            DirtyClasses();
            DirtyStyles();

            InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size based on the theme settings.
        /// </summary>
        protected Size ThemeSize => Size ?? Theme?.PaginationOptions?.Size ?? Blazorise.Size.None;

        /// <summary>
        /// Gets or sets the pagination size.
        /// </summary>
        [Parameter]
        public Size? Size
        {
            get => size;
            set
            {
                size = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the pagination alignment.
        /// </summary>
        [Parameter]
        public Alignment Alignment
        {
            get => alignment;
            set
            {
                alignment = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Pagination"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Cascaded theme settings.
        /// </summary>
        [CascadingParameter] public Theme Theme { get; set; }

        #endregion
    }
}
