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
/// Renders a report table element on the designer or viewer surface.
/// </summary>
public partial class _ReportDesignerTable
{
    /// <summary>
    /// Table element definition rendered in the designer or viewer.
    /// </summary>
    [Parameter] public ReportElementDefinition Element { get; set; }
}