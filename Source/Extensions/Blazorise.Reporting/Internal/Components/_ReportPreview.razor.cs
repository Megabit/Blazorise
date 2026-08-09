#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the active report preview format.
/// </summary>
public partial class _ReportPreview
{
    #region Properties

    /// <summary>
    /// Designer that supplies preview state and commands.
    /// </summary>
    [Parameter, EditorRequired] public _ReportDesigner Designer { get; set; }

    #endregion
}