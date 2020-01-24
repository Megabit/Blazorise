#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Tooltip : BaseComponent
    {
        #region Members

        private Placement placement = Placement.Top;

        private bool isMultiline;

        private bool isAlwaysActive;

        private bool isInlined;

        private bool isFade;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Tooltip() );
            builder.Append( ClassProvider.TooltipPlacement( Placement ) );
            builder.Append( ClassProvider.TooltipMultiline(), IsMultiline );
            builder.Append( ClassProvider.TooltipAlwaysActive(), IsAlwaysActive );
            builder.Append( ClassProvider.TooltipInline(), IsInline );
            builder.Append( ClassProvider.TooltipFade(), IsFade );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( !IsInline )
            {
                // try to detect if inline is needed
                ExecuteAfterRender( async () =>
                {
                    await JSRunner.InitializeTooltip( ElementRef, ElementId );
                } );
            }

            base.OnInitialized();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a regular tooltip's content. 
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// Gets or sets the tooltip location relative to it's component.
        /// </summary>
        [Parameter]
        public Placement Placement
        {
            get => placement;
            set
            {
                placement = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Force the multiline display.
        /// </summary>
        [Parameter]
        public bool IsMultiline
        {
            get => isMultiline;
            set
            {
                isMultiline = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Always show tooltip, instead of just when hovering over the element.
        /// </summary>
        [Parameter]
        public bool IsAlwaysActive
        {
            get => isAlwaysActive;
            set
            {
                isAlwaysActive = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Force inline block instead of trying to detect the element block.
        /// </summary>
        [Parameter]
        public bool IsInline
        {
            get => isInlined;
            set
            {
                isInlined = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool IsFade
        {
            get => isFade;
            set
            {
                isFade = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
