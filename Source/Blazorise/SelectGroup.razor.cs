#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class SelectGroup : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the group label.
        /// </summary>
        [Parameter] public string Label { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
