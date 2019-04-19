#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseProgressBar : BaseComponent
    {
        #region Members

        private Background background = Background.None;

        private bool isStriped;

        private bool isAnimated;

        private int? @value;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.ProgressBar() )
                .Add( () => ClassProvider.ProgressBarWidth( Value ?? 0 ) )
                .If( () => ClassProvider.ProgressBarColor( Background ), () => Background != Background.None )
                .If( () => ClassProvider.ProgressBarStriped(), () => IsStriped )
                .If( () => ClassProvider.ProgressBarAnimated(), () => IsAnimated );

            base.RegisterClasses();
        }

        protected override void RegisterStyles()
        {
            StyleMapper
                .If( () => StyleProvider.ProgressBarValue( Value ?? 0 ), () => Value != null );

            base.RegisterStyles();
        }

        public void Animate( bool isAnimated )
        {
            IsAnimated = isAnimated;
            StateHasChanged();
        }

        #endregion

        #region Properties

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

        [Parameter]
        protected bool IsStriped
        {
            get => isStriped;
            set
            {
                isStriped = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected bool IsAnimated
        {
            get => isAnimated;
            set
            {
                isAnimated = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected int Min { get; set; } = 0;

        [Parameter] protected int Max { get; set; } = 100;

        [Parameter]
        protected int? Value
        {
            get => @value;
            set
            {
                this.@value = value;

                ClassMapper.Dirty();
                StyleMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
