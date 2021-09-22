#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Toggle visibility of almost any content on your pages in a vertically collapsing container.
    /// </summary>
    public partial class Collapse : BaseComponent
    {
        #region Members

        private bool visible;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Collapse( InsideAccordion ) );
            builder.Append( ClassProvider.CollapseActive( InsideAccordion, Visible ) );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Toggles the collapse visibility state.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Toggle()
        {
            Visible = !Visible;

            return InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines if the collapse is placed inside of accordion component.
        /// </summary>
        public bool InsideAccordion => ParentAccordion != null;

        /// <summary>
        /// Gets or sets the collapse visibility state.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the cascaded parent accordion component.
        /// </summary>
        [CascadingParameter] protected Accordion ParentAccordion { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Collapse"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
