#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components
{
    public partial class DocsAttributes
    {
        #region Properties

        [Parameter] public string Title { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
