﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public partial class PaginationLink : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.PaginationLink() );
            builder.Append( ClassProvider.PaginationLinkActive(), ParentPaginationItem?.IsActive == true );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler( MouseEventArgs eventArgs )
        {
            return Clicked.InvokeAsync( Page );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the page name.
        /// </summary>
        [Parameter]
        public string Page { get; set; }

        /// <summary>
        /// Occurs when the item link is clicked.
        /// </summary>
        [Parameter] public EventCallback<string> Clicked { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] protected PaginationItem ParentPaginationItem { get; set; }

        #endregion
    }
}
