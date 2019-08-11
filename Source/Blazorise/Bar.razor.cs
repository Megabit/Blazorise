#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseBar : BaseComponent
    {
        #region Members

        private Breakpoint breakpoint = Breakpoint.None;

        private ThemeContrast themeContrast = ThemeContrast.Light;

        private Alignment alignment = Alignment.None;

        private Background background = Background.None;

        private BaseBarToggler barToggler;

        private BaseBarMenu barMenu;

        private bool isOpen;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
               .Add( () => ClassProvider.Bar() )
               .If( () => ClassProvider.BackgroundColor( Background ), () => Background != Background.None )
               .If( () => ClassProvider.BarThemeContrast( ThemeContrast ), () => ThemeContrast != ThemeContrast.None )
               .If( () => ClassProvider.BarBreakpoint( Breakpoint ), () => Breakpoint != Breakpoint.None )
               .If( () => ClassProvider.FlexAlignment( Alignment ), () => Alignment != Alignment.None );

            base.RegisterClasses();
        }

        internal void Hook( BaseBarToggler barToggler )
        {
            this.barToggler = barToggler;
        }

        internal void Hook( BaseBarMenu barMenu )
        {
            this.barMenu = barMenu;
        }

        internal void Toggle()
        {
            IsOpen = !IsOpen;

            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Controlls the state of toggler and the menu.
        /// </summary>
        [Parameter]
        internal protected bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                if ( barMenu != null )
                    barMenu.IsOpen = value;

                if ( barToggler != null )
                    barToggler.IsOpen = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Used for responsive collapsing.
        /// </summary>
        [Parameter]
        protected Breakpoint Breakpoint
        {
            get => breakpoint;
            set
            {
                breakpoint = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected ThemeContrast ThemeContrast
        {
            get => themeContrast;
            set
            {
                themeContrast = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Defines the alignment within bar.
        /// </summary>
        [Parameter]
        protected Alignment Alignment
        {
            get => alignment;
            set
            {
                alignment = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Gets or sets the bar background color.
        /// </summary>
        [Parameter]
        protected Background Background
        {
            get => background;
            set
            {
                background = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
