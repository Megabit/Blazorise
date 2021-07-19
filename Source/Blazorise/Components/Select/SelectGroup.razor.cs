#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Group item in the <see cref="Select{TValue}"/> component.
    /// </summary>
    public partial class SelectGroup : BaseComponent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the group label.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="SelectGroup"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
