#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseBarDropdownItem : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.BarDropdownItem() );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] private EventCallback Clicked { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        [Parameter] protected string To { get; set; }

        [Parameter] protected Match Match { get; set; } = Match.All;

        [Parameter] protected string Title { get; set; }

        #endregion
    }
}
