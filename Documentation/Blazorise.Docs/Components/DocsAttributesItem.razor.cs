#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components
{
    public partial class DocsAttributesItem
    {
        #region Properties

        [Parameter] public string Name { get; set; }

        [Parameter] public string Type { get; set; }

        [Parameter] public bool TypeTag { get; set; }

        [Parameter] public string Default { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
