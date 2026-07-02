#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Groups related report designer property editors.
/// </summary>
public partial class _ReportDesignerPropertyGroup
{
    /// <summary>
    /// Group heading text.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Property editor content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}