#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components
{
    public partial class DocsPageHeader
    {
        #region Properties

        [Parameter] public string Title { get; set; }

        [Parameter] public string Keywords { get; set; }

        [Parameter] public RenderFragment SubTitle { get; set; }

        [Parameter] public RenderFragment Description { get; set; }

        #endregion        
    }
}
