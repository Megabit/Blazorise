#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Owns report designer dialogs and their component references.
/// </summary>
public partial class _ReportDesignerDialogsHost
{
    #region Members

    private _ReportDesignerAggregateDialog aggregateDialog;

    private _ReportDesignerRunningTotalDialog runningTotalDialog;

    private _ReportDesignerGroupDialog groupDialog;

    private _ReportDesignerDataSourceConnectionDialog dataSourceConnectionDialog;

    private _ReportDesignerFormulaDialog formulaDialog;

    #endregion

    #region Methods

    internal Task ShowAggregate( IEnumerable<ReportDesignerFieldOption> fieldOptions, string selectedFieldName, IEnumerable<ReportAggregateSummaryLocation> summaryLocations )
        => aggregateDialog?.Show( fieldOptions, selectedFieldName, summaryLocations ) ?? Task.CompletedTask;

    internal Task ShowRunningTotal( ReportRunningTotalDefinition runningTotal )
        => runningTotalDialog?.Show( runningTotal ) ?? Task.CompletedTask;

    internal Task ShowGroup( IEnumerable<ReportDesignerFieldOption> fieldOptions, string selectedFieldName )
        => groupDialog?.Show( fieldOptions, selectedFieldName ) ?? Task.CompletedTask;

    internal Task ShowDataSourceConnection( ReportDefinition definition, IEnumerable<IReportDataSourceProvider> providers )
        => dataSourceConnectionDialog?.Show( definition, providers ) ?? Task.CompletedTask;

    internal Task ShowFormula( string propertyName, string formula )
        => formulaDialog?.Show( propertyName, formula ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    [Parameter, EditorRequired] public _ReportDesigner Designer { get; set; }

    [Parameter, EditorRequired] public ReportDefinition Definition { get; set; }

    #endregion
}