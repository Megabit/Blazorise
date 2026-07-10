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
/// Declares a group footer band rendered after grouped detail rows.
/// </summary>
public partial class ReportGroupFooter
{
    protected override ReportBandType SectionType => ReportBandType.GroupFooter;
}