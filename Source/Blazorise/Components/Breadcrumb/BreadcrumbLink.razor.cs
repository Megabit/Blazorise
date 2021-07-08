﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Links can be href's for anchor tags, or to's for router-links.
    /// </summary>
    public partial class BreadcrumbLink : BaseComponent
    {
        #region Members

        private bool disabled;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BreadcrumbLink() );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            ParentBreadcrumbItem?.NotifyRelativeUriChanged( To );

            base.OnInitialized();
        }

        /// <summary>
        /// Handles the link onclick event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task ClickHandler()
        {
            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if link should be in active state.
        /// </summary>
        protected bool IsActive => ParentBreadcrumbItem?.Active == true;

        /// <summary>
        /// When set to 'true', disables the component's functionality and places it in a disabled state.
        /// </summary>
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

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="BreadcrumbLink"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="BreadcrumbItem"/> component.
        /// </summary>
        [CascadingParameter] protected BreadcrumbItem ParentBreadcrumbItem { get; set; }

        #endregion
    }
}
