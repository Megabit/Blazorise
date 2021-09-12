#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components
{
    public partial class DocsAttributesItem
    {
        #region Properties

        private string DefaultClassNames => Default switch
        {
            "true" => "b-attribute-token boolean",
            "false" => "b-attribute-token boolean",
            "null" => "b-attribute-token keyword",
            "None" => "b-attribute-token keyword",
            _ => "b-attribute-token string",
        };

        [Parameter] public string Name { get; set; }

        [Parameter] public string Type { get; set; }

        [Parameter] public bool TypeTag { get; set; }

        [Parameter] public string Default { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
