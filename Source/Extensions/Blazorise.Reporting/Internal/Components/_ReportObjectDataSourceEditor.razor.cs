#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal editor for object model data source settings.
/// </summary>
public partial class _ReportObjectDataSourceEditor
{
    #region Properties

    /// <summary>
    /// Provider settings context edited by the object data source editor.
    /// </summary>
    [Parameter] public ReportDataSourceProviderEditorContext Context { get; set; }

    #endregion
}