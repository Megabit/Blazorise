#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Bar : BaseComponent
    {
        #region Members

        private Breakpoint breakpoint = Breakpoint.None;

        private ThemeContrast themeContrast = ThemeContrast.Light;

        private Alignment alignment = Alignment.None;

        private Background background = Background.None;

        private bool visible;

        public event EventHandler<BarStateEventArgs> StateChanged;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Bar() );
            builder.Append( ClassProvider.BackgroundColor( Background ), Background != Background.None );
            builder.Append( ClassProvider.BarThemeContrast( ThemeContrast ), ThemeContrast != ThemeContrast.None );
            builder.Append( ClassProvider.BarBreakpoint( Breakpoint ), Breakpoint != Breakpoint.None );
            builder.Append( ClassProvider.FlexAlignment( Alignment ), Alignment != Alignment.None );

            base.BuildClasses( builder );
        }

        internal void Toggle()
        {
            Visible = !Visible;

            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Controlls the state of toggler and the menu.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                // prevent bar from calling the same code multiple times
                if ( value == visible )
                    return;

                visible = value;

                StateChanged?.Invoke( this, new BarStateEventArgs( visible ) );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Used for responsive collapsing.
        /// </summary>
        [Parameter]
        public Breakpoint Breakpoint
        {
            get => breakpoint;
            set
            {
                breakpoint = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public ThemeContrast ThemeContrast
        {
            get => themeContrast;
            set
            {
                themeContrast = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the alignment within bar.
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
        /// Gets or sets the bar background color.
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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
