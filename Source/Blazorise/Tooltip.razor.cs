#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseTooltip : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reference to the arrow element.
        /// </summary>
        public ElementReference ArrowRef { get; protected set; }

        /// <summary>
        /// Gets or sets a regular tooltip's content. 
        /// </summary>
        [Parameter] public string Text { get; set; }

        [Parameter] public RenderFragment HtmlTemplate { get; set; }

        /// <summary>
        /// Gets or sets the tooltip location relative to it's component.
        /// </summary>
        [Parameter] public Placement Placement { get; set; } = Placement.Top;

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
