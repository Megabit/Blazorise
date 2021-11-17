#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Wrapper for a regular html form element.
    /// </summary>
    public partial class Form : BaseComponent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="Form"/> component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
