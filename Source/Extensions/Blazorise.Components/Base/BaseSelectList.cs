#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components.Base
{
    public abstract class BaseSelectList<TItem> : ComponentBase
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        [Parameter] protected IEnumerable<TItem> Data { get; set; }

        [Parameter] protected Func<TItem, string> TextField { get; set; }

        [Parameter] protected Func<TItem, object> ValueField { get; set; }

        [Parameter] protected object SelectedValue { get; set; }

        [Parameter] protected Action<object> SelectedValueChanged { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
