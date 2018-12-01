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
    public abstract class BaseBarDropdownToggler : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.BarDropdownToggler() );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            // link to the parent component
            BarItem?.Hook( this );

            base.OnInit();
        }

        protected void ClickHandler()
        {
            BarItem?.Toggle();
        }

        #endregion

        #region Properties

        [CascadingParameter] protected BarItem BarItem { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
