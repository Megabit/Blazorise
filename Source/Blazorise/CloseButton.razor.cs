#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CloseButton : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CloseButton() );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler()
        {
            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        [CascadingParameter] protected Alert ParentAlert { get; set; }

        [CascadingParameter] protected Modal ParentModal { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
