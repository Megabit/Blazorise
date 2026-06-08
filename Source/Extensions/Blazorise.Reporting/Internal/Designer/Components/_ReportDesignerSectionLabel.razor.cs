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
/// Renders the band label used by the classic report designer surface.
/// </summary>
public partial class _ReportDesignerSectionLabel
{
    private string Label => $"{ReportDefinitionHelper.GetSectionTypeDisplayName( Section.Type )}: {ReportDefinitionHelper.GetSectionDisplayName( Section )}";

    /// <summary>
    /// Report section displayed in the designer label.
    /// </summary>
    [Parameter] public ReportSectionDefinition Section { get; set; }
}