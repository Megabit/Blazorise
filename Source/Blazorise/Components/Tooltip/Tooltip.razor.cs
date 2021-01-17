#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Tooltip : BaseComponent
    {
        #region Members

        private Placement placement = Placement.Top;

        private bool multiline;

        private bool alwaysActive;

        private bool inline;

        private bool fade;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Tooltip() );
            builder.Append( ClassProvider.TooltipPlacement( Placement ) );
            builder.Append( ClassProvider.TooltipMultiline(), Multiline );
            builder.Append( ClassProvider.TooltipAlwaysActive(), AlwaysActive );
            builder.Append( ClassProvider.TooltipInline(), Inline );
            builder.Append( ClassProvider.TooltipFade(), Fade );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( !Inline )
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

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

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
        public bool Multiline
        {
            get => multiline;
            set
            {
                multiline = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Always show tooltip, instead of just when hovering over the element.
        /// </summary>
        [Parameter]
        public bool AlwaysActive
        {
            get => alwaysActive;
            set
            {
                alwaysActive = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Force inline block instead of trying to detect the element block.
        /// </summary>
        [Parameter]
        public bool Inline
        {
            get => inline;
            set
            {
                inline = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the tooltip fade transition.
        /// </summary>
        [Parameter]
        public bool Fade
        {
            get => fade;
            set
            {
                fade = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
