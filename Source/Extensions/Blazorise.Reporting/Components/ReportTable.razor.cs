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

namespace Blazorise.Reporting;

/// <summary>
/// Declares a table element used to organize report fields and text.
/// </summary>
public partial class ReportTable
{
    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Table;

    /// <summary>
    /// Columns declared inside the report table.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}