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
/// Declares a page header band rendered at the top of each report page.
/// </summary>
public partial class ReportPageHeader
{
    #region Properties

    protected override ReportBandType SectionType => ReportBandType.PageHeader;

    #endregion
}