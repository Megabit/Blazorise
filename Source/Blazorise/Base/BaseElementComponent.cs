#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base class for all basic html elements.
    /// </summary>
    public class BaseElementComponent : BaseComponent
    {
        #region Properties

        /// <summary>
        /// Specifies the content to be rendered inside this component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
