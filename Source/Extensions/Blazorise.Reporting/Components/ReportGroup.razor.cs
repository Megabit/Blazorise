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
/// Declares a legacy grouped band in a report definition.
/// </summary>
public partial class ReportGroup
{
    protected override ReportSectionType SectionType => ReportSectionType.GroupHeader;
}