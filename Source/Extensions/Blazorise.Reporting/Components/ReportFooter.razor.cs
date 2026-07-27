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
/// Declares the report footer band rendered once at the end of the report.
/// </summary>
public partial class ReportFooter
{
    #region Properties

    protected override ReportBandType SectionType => ReportBandType.ReportFooter;

    #endregion
}