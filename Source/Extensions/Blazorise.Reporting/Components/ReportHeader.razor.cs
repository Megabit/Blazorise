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
/// Declares the report header band rendered once at the beginning of the report.
/// </summary>
public partial class ReportHeader
{
    #region Properties

    protected override ReportBandType SectionType => ReportBandType.ReportHeader;

    #endregion
}