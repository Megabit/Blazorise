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

    /// <summary>
    /// Designer that supplies preview context.
    /// </summary>
    [Parameter, EditorRequired] public _ReportDesigner Designer { get; set; }

    /// <summary>
    /// Report definition used to render the sections.
    /// </summary>
    [Parameter, EditorRequired] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Preview sections rendered by this component.
    /// </summary>
    [Parameter, EditorRequired] public IReadOnlyList<TSection> Sections { get; set; }

    /// <summary>
    /// Page number associated with the section collection.
    /// </summary>
    [Parameter] public int PageNumber { get; set; }

    /// <summary>
    /// Suffix used to keep rendered section keys distinct.
    /// </summary>
    [Parameter] public string KeySuffix { get; set; }

    #endregion
}