#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Base component for custom report element renderers.
/// </summary>
public abstract class BaseReportElementRenderer : ComponentBase
{
    #region Properties

    /// <summary>
    /// Gets or sets the custom element render context.
    /// </summary>
    [Parameter, EditorRequired] public ReportElementRenderContext Context { get; set; }

    #endregion
}