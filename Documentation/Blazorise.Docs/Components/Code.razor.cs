#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components
{
    public partial class Code
    {
        #region Properties

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public bool SecondaryColor { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public bool Tag { get; set; }

        #endregion
    }
}
