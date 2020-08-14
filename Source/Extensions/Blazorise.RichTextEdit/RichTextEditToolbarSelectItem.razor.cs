#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEditToolbarSelectItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this option is selected, eg the default value.
        /// </summary>
        [Parameter] public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the value of this option.
        /// </summary>
        [Parameter] public string Value { get; set; }

        /// <summary>
        /// Gets or sets the child content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
