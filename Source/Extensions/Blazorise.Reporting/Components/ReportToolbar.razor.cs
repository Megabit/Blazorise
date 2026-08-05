#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares custom toolbar content for the containing report.
/// </summary>
public partial class ReportToolbar : ComponentBase, IDisposable
{
    #region Members

    private ReportContext registeredReportContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool optionsChanged = registeredReportContext is null
            || parameters.IsParameterChanged( ChildContent )
            || parameters.IsParameterChanged( ButtonTemplate )
            || parameters.TryGetParameter( HiddenCommands,
                value => value is null
                    ? HiddenCommands is null
                    : HiddenCommands is not null && value.SequenceEqual( HiddenCommands ),
                out ComponentParameterInfo<IReadOnlyCollection<ReportCommand>> hiddenCommandsParameter ) && hiddenCommandsParameter.Changed
            || parameters.IsParameterChanged( ShowPanesMenu )
            || parameters.IsParameterChanged( ShowPersistenceButtons )
            || parameters.IsParameterChanged( ShowEditButtons )
            || parameters.IsParameterChanged( ShowHistoryButtons )
            || parameters.IsParameterChanged( ShowDataSourceButtons )
            || parameters.IsParameterChanged( ShowExportButtons )
            || parameters.IsParameterChanged( ShowModeButtons );

        await base.SetParametersAsync( parameters );

        bool contextChanged = !ReferenceEquals( registeredReportContext, ReportContext );

        if ( contextChanged )
        {
            registeredReportContext?.UnregisterToolbar( this );
            registeredReportContext = ReportContext;
        }

        if ( optionsChanged || contextChanged )
            registeredReportContext?.RegisterToolbar( this, this );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredReportContext?.UnregisterToolbar( this );
        registeredReportContext = null;
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
    /// Shows the save and load buttons in the default report toolbar.
    /// </summary>
    [Parameter] public bool ShowPersistenceButtons { get; set; } = true;

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