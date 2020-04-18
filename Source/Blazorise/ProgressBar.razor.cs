#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class ProgressBar : BaseComponent
    {
        #region Members

        private Background background = Background.None;

        private bool striped;

        private bool animated;

        private int? @value;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ProgressBar() );
            builder.Append( ClassProvider.ProgressBarWidth( Value ?? 0 ) );
            builder.Append( ClassProvider.ProgressBarColor( Background ), Background != Background.None );
            builder.Append( ClassProvider.ProgressBarStriped(), Striped );
            builder.Append( ClassProvider.ProgressBarAnimated(), Animated );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            if ( Value != null )
                builder.Append( StyleProvider.ProgressBarValue( Value ?? 0 ) );

            builder.Append( StyleProvider.ProgressBarSize( ParentProgress?.Size ?? Size.None ) );

            base.BuildStyles( builder );
        }

        public void Animate( bool animated )
        {
            Animated = animated;
            StateHasChanged();
        }

        #endregion

        #region Properties

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

        [Parameter] public int Min { get; set; } = 0;

        [Parameter] public int Max { get; set; } = 100;

        [Parameter]
        public int? Value
        {
            get => @value;
            set
            {
                this.@value = value;

                DirtyClasses();
                DirtyStyles();
            }
        }

        [CascadingParameter] protected Progress ParentProgress { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
