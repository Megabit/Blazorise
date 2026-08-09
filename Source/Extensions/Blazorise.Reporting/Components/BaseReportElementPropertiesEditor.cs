#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Base component for custom report element properties editors.
/// </summary>
public abstract class BaseReportElementPropertiesEditor : ComponentBase
{
    #region Properties

    /// <summary>
    /// Gets or sets the custom element properties context.
    /// </summary>
    [Parameter, EditorRequired] public ReportElementPropertiesContext Context { get; set; }

    #endregion
}