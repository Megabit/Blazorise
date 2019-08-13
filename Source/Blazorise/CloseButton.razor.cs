#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseCloseButton : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.CloseButton() );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] private EventCallback Clicked { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
