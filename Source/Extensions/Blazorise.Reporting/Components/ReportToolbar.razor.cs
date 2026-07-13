#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares custom toolbar content for the containing report.
/// </summary>
public partial class ReportToolbar : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        ReportContext?.RegisterToolbar( this );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Toolbar items rendered by the report toolbar area.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Template used to render command buttons in the report toolbar.
    /// </summary>
    [Parameter] public RenderFragment<ReportToolbarItemContext> ButtonTemplate { get; set; }

    /// <summary>
    /// Commands omitted from the default report toolbar.
    /// </summary>
    [Parameter] public IReadOnlyCollection<ReportCommand> HiddenCommands { get; set; }

    /// <summary>
    /// Shows the panes menu in the default report toolbar when dock panes are available.
    /// </summary>
    [Parameter] public bool ShowPanesMenu { get; set; } = true;

    /// <summary>
    /// Shows the edit command buttons in the default report toolbar.
    /// </summary>
    [Parameter] public bool ShowEditButtons { get; set; } = true;

    /// <summary>
    /// Shows the history command buttons in the default report toolbar.
    /// </summary>
    [Parameter] public bool ShowHistoryButtons { get; set; } = true;

    /// <summary>
    /// Shows the data source command buttons in the default report toolbar.
    /// </summary>
    [Parameter] public bool ShowDataSourceButtons { get; set; } = true;

    /// <summary>
    /// Shows the export command buttons in the default report toolbar.
    /// </summary>
    [Parameter] public bool ShowExportButtons { get; set; } = true;

    /// <summary>
    /// Shows the report mode buttons in the default report toolbar.
    /// </summary>
    [Parameter] public bool ShowModeButtons { get; set; } = true;

    #endregion
}