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
/// Declares a detail band repeated for records from its data source.
/// </summary>
public partial class ReportDetail
{
    protected override ReportBandType SectionType => ReportBandType.Detail;
}