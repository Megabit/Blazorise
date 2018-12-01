#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseBar : BaseComponent
    {
        #region Members

        private Breakpoint breakpoint = Breakpoint.None;

        private Theme theme = Theme.Light;

        private Alignment alignment = Alignment.None;

        private Background background = Background.None;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
               .Add( () => ClassProvider.Bar() )
               .If( () => ClassProvider.BackgroundColor( Background ), () => Background != Background.None )
               .If( () => ClassProvider.BarShade( Theme ), () => Theme != Theme.None )
               .If( () => ClassProvider.BarBreakpoint( Breakpoint ), () => Breakpoint != Breakpoint.None )
               .If( () => ClassProvider.FlexAlignment( Alignment ), () => Alignment != Alignment.None );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

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
        protected Theme Theme
        {
            get => theme;
            set
            {
                theme = value;

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
