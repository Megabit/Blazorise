#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Owns the docked report designer workspace and its interactive component references.
/// </summary>
public partial class _ReportDesignerWorkspace
{
    #region Members

    private _ReportDesignerLayout designerLayout;

    private _ReportDesignerSurface designerSurface;

    private _ReportDesignerContextMenu contextMenu;

    private _ReportDesignerDialogsHost dialogsHost;

    #endregion

    #region Methods

    internal Task CapturePaneScrollPositions( Dictionary<string, (double Left, double Top)> positions )
        => designerLayout?.CapturePaneScrollPositions( positions ) ?? Task.CompletedTask;

    internal void RefreshSurface()
    {
        if ( designerLayout is not null )
            _ = designerLayout.RefreshSurface();
    }

    internal void BeginFieldDrag( string dataSourceName, string fieldName )
        => designerSurface?.BeginFieldDrag( dataSourceName, fieldName );

    internal void BeginToolboxElementDrag( ReportElementType elementType, string text )
        => designerSurface?.BeginToolboxElementDrag( elementType, text );

    internal Task CompleteExternalDrag()
        => designerSurface?.CompleteExternalDrag() ?? Task.CompletedTask;

    internal Task ShowContextMenu( ReportContextMenuState state )
        => contextMenu?.Show( state ) ?? Task.CompletedTask;

    internal Task CloseContextMenu()
        => contextMenu?.CloseMenu() ?? Task.CompletedTask;

    internal Task ShowPropertiesPane()
        => designerLayout?.ShowPropertiesPane() ?? Task.CompletedTask;

    internal Task ShowAggregateDialog( IEnumerable<ReportDesignerFieldOption> fields, string selectedFieldName, IEnumerable<ReportAggregateSummaryLocation> summaryLocations )
        => dialogsHost?.ShowAggregate( fields, selectedFieldName, summaryLocations ) ?? Task.CompletedTask;

    internal Task ShowRunningTotalDialog( ReportRunningTotalDefinition runningTotal )
        => dialogsHost?.ShowRunningTotal( runningTotal ) ?? Task.CompletedTask;

    internal Task ShowGroupDialog( IEnumerable<ReportDesignerFieldOption> fields, string selectedFieldName )
        => dialogsHost?.ShowGroup( fields, selectedFieldName ) ?? Task.CompletedTask;

    internal Task ShowDataSourceConnectionDialog( ReportDefinition definition, IEnumerable<IReportDataSourceProvider> providers )
        => dialogsHost?.ShowDataSourceConnection( definition, providers ) ?? Task.CompletedTask;

    internal Task ShowFormulaDialog( string propertyName, string formula )
        => dialogsHost?.ShowFormula( propertyName, formula ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    [Parameter, EditorRequired] public _ReportDesigner Designer { get; set; }

    #endregion
}