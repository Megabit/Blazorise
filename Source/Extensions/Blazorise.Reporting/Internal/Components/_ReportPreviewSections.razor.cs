#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders one collection of report preview sections.
/// </summary>
public partial class _ReportPreviewSections<TSection>
{
    #region Properties

    [Parameter, EditorRequired] public _ReportDesigner Designer { get; set; }

    [Parameter, EditorRequired] public ReportDefinition Definition { get; set; }

    [Parameter, EditorRequired] public IReadOnlyList<TSection> Sections { get; set; }

    [Parameter] public int PageNumber { get; set; }

    [Parameter] public string KeySuffix { get; set; }

    #endregion
}