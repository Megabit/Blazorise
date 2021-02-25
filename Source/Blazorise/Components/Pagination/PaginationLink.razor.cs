﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Clickable element for page numbers.
    /// </summary>
    public partial class PaginationLink : BaseComponent
    {
        #region Members

        /// <summary>
        /// Holds the state of the parent item state object.
        /// </summary>
        private PaginationItemState parentPaginationItemState;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.PaginationLink() );
            builder.Append( ClassProvider.PaginationLinkActive(), ParentPaginationItemState.Active );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the link onclick event.
        /// </summary>
        /// <param name="eventArgs">Information about the mouse event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task ClickHandler( MouseEventArgs eventArgs )
        {
            return Clicked.InvokeAsync( Page );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the page name.
        /// </summary>
        [Parameter] public string Page { get; set; }

        /// <summary>
        /// Occurs when the item link is clicked.
        /// </summary>
        [Parameter] public EventCallback<string> Clicked { get; set; }

        /// <summary>
        /// Cascaded <see cref="PaginationItemState"/> for the <see cref="PaginationItem"/> in which this link is placed.
        /// </summary>
        [CascadingParameter]
        protected PaginationItemState ParentPaginationItemState
        {
            get => parentPaginationItemState;
            set
            {
                if ( parentPaginationItemState == value )
                    return;

                parentPaginationItemState = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="PaginationLink"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
