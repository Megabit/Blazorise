#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BreadcrumbLink : BaseComponent
    {
        #region Members

        private bool disabled;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BreadcrumbLink() );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( ParentBreadcrumbItem != null )
            {
                ParentBreadcrumbItem.NotifyRelativeUriChanged( To );
            }

            base.OnInitialized();
        }

        protected Task ClickHandler()
        {
            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        protected bool IsActive => ParentBreadcrumbItem?.Active == true;

        [Parameter]
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Link to the destination page.
        /// </summary>
        [Parameter] public string To { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the linked document.
        /// </summary>
        [Parameter] public Target Target { get; set; } = Target.None;

        /// <summary>
        /// URL matching behavior for a link.
        /// </summary>
        [Parameter] public Match Match { get; set; } = Match.All;

        /// <summary>
        /// Defines the title of a link, which appears to the user as a tooltip.
        /// </summary>
        [Parameter] public string Title { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] protected BreadcrumbItem ParentBreadcrumbItem { get; set; }

        #endregion
    }
}
