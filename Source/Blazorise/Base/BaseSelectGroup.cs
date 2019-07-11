#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseSelectGroup : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        [Parameter] protected string Label { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
