#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseDropdownItem : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.DropdownItem() );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            Clicked.InvokeAsync( Value );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Holds the item value.
        /// </summary>
        [Parameter] protected object Value { get; set; }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] protected EventCallback<object> Clicked { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
