﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A wrapper for collapse header.
    /// </summary>
    public partial class CollapseHeader : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CollapseHeader() );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the header onclick event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task ClickHandler()
        {
            ParentCollapse.Toggle();

            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the header is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="CollapseHeader"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="Collapse"/> component.
        /// </summary>
        [CascadingParameter] protected Collapse ParentCollapse { get; set; }

        #endregion
    }
}
