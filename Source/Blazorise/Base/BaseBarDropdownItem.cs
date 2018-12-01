#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
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
            Clicked?.Invoke();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] private Action Clicked { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
