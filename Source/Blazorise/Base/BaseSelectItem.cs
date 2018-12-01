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

        #endregion

        #region Properties

        [Parameter] protected string Value { get; set; }

        [Parameter] protected bool IsSelected { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
