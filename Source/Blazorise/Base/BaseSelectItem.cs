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
    public abstract class BaseSelectItem : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void OnInit()
        {
            ParentSelect?.Register( this );

            base.OnInit();
        }

        #endregion

        #region Properties

        protected bool IsSelected => ParentSelect?.IsSelected( this ) == true;

        [Parameter] internal protected string Value { get; set; }

        [CascadingParameter] protected BaseSelect ParentSelect { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
