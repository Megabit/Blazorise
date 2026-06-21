#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders report source fields, formula fields, running totals, and special fields.
/// </summary>
public partial class _ReportDesignerFieldsExplorerPanel
{
    #region Members

    private string pendingFormulaFieldName;

    private string pendingFormulaFieldRenameSourceName;

    private string selectedFormulaFieldName;

    private string selectedRunningTotalName;

    private ReportFormulaFieldNameDialogMode formulaFieldNameDialogMode;

    private ReportRunningTotalNameDialogMode runningTotalNameDialogMode;

    private ReportFormulaFieldContextMenuState formulaFieldContextMenu;

    private ReportRunningTotalContextMenuState runningTotalContextMenu;

    private ReportDataSourceContextMenuState dataSourceContextMenu;

    private _ReportDesignerFormulaDialog formulaDialogRef;

    private _ReportDesignerFormulaFieldNameDialog formulaFieldNameDialogRef;

    private _ReportDesignerFormulaFieldNameDialog runningTotalNameDialogRef;

    private _ReportDesignerRunningTotalDialog runningTotalDialogRef;

    private string pendingRunningTotalRenameSourceName;

    #endregion

    #region Methods

    private async Task FieldsNodeClicked( ReportTreeNode node )
    {
        if ( node?.Kind == ReportTreeNodeKind.FormulaField )
            selectedFormulaFieldName = node.Text;

        if ( node?.Kind == ReportTreeNodeKind.RunningTotalField )
            selectedRunningTotalName = node.Text;

        await CloseFormulaFieldContextMenu();
        await CloseRunningTotalContextMenu();
        await CloseDataSourceContextMenu();
    }

    private async Task FieldsNodeDoubleClicked( ReportTreeNode node )
    {
        if ( node?.Kind == ReportTreeNodeKind.FormulaField )
        {
            selectedFormulaFieldName = node.Text;
            await CloseFormulaFieldContextMenu();
            await CloseRunningTotalContextMenu();
            await CloseDataSourceContextMenu();
            await FormulaFieldInserted.InvokeAsync( node.Text );
            return;
        }

        if ( node?.Kind == ReportTreeNodeKind.RunningTotalField )
        {
            selectedRunningTotalName = node.Text;
            await CloseFormulaFieldContextMenu();
            await CloseRunningTotalContextMenu();
            await CloseDataSourceContextMenu();
            await RunningTotalInserted.InvokeAsync( node.Text );
            return;
        }

        if ( node?.Value is ReportFieldTreeNodeValue field )
        {
            await CloseFormulaFieldContextMenu();
            await CloseRunningTotalContextMenu();
            await CloseDataSourceContextMenu();
            await FieldInserted.InvokeAsync( (field.DataSourceName, field.FieldName) );
        }
    }

    private Task FieldsNodeContextMenu( ReportTreeNodeMouseEventArgs eventArgs )
    {
        if ( eventArgs?.Node?.Key == ReportDesignerTreeBuilder.FormulaFieldsNodeKey
            || eventArgs?.Node?.Kind == ReportTreeNodeKind.FormulaField )
        {
            selectedFormulaFieldName = eventArgs.Node.Kind == ReportTreeNodeKind.FormulaField ? eventArgs.Node.Text : selectedFormulaFieldName;
            formulaFieldContextMenu = new()
            {
                Visible = true,
                FormulaFieldName = eventArgs.Node.Kind == ReportTreeNodeKind.FormulaField ? eventArgs.Node.Text : null,
                ClientX = eventArgs.MouseEventArgs.ClientX,
                ClientY = eventArgs.MouseEventArgs.ClientY,
            };
            runningTotalContextMenu = null;
            dataSourceContextMenu = null;
        }
        else if ( eventArgs?.Node?.Key == ReportDesignerTreeBuilder.RunningTotalFieldsNodeKey
            || eventArgs?.Node?.Kind == ReportTreeNodeKind.RunningTotalField )
        {
            selectedRunningTotalName = eventArgs.Node.Kind == ReportTreeNodeKind.RunningTotalField ? eventArgs.Node.Text : selectedRunningTotalName;
            runningTotalContextMenu = new()
            {
                Visible = true,
                RunningTotalName = eventArgs.Node.Kind == ReportTreeNodeKind.RunningTotalField ? eventArgs.Node.Text : null,
                ClientX = eventArgs.MouseEventArgs.ClientX,
                ClientY = eventArgs.MouseEventArgs.ClientY,
            };
            formulaFieldContextMenu = null;
            dataSourceContextMenu = null;
        }
        else if ( eventArgs?.Node?.Value is ReportDataSourceTreeNodeValue dataSource )
        {
            dataSourceContextMenu = new()
            {
                Visible = true,
                DataSourceName = dataSource.DataSourceName,
                ClientX = eventArgs.MouseEventArgs.ClientX,
                ClientY = eventArgs.MouseEventArgs.ClientY,
            };
            formulaFieldContextMenu = null;
            runningTotalContextMenu = null;
        }

        return Task.CompletedTask;
    }

    private async Task FormulaFieldNameConfirmed( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        string confirmedName = formulaFieldName.Trim();

        if ( formulaFieldNameDialogMode == ReportFormulaFieldNameDialogMode.Rename )
        {
            await FormulaFieldRenamed.InvokeAsync( (pendingFormulaFieldRenameSourceName, confirmedName) );
            selectedFormulaFieldName = confirmedName;
            pendingFormulaFieldRenameSourceName = null;
            return;
        }

        pendingFormulaFieldName = confirmedName;
        selectedFormulaFieldName = confirmedName;

        await formulaDialogRef.ShowAsync( pendingFormulaFieldName, null );
    }

    private async Task FormulaFieldFormulaConfirmed( string formula )
    {
        if ( string.IsNullOrWhiteSpace( pendingFormulaFieldName ) )
            return;

        await FormulaFieldConfirmed.InvokeAsync( new()
        {
            Name = pendingFormulaFieldName,
            Formula = formula,
        } );
    }

    private async Task RunningTotalNameConfirmed( string runningTotalName )
    {
        if ( string.IsNullOrWhiteSpace( runningTotalName ) )
            return;

        string confirmedName = runningTotalName.Trim();

        if ( runningTotalNameDialogMode == ReportRunningTotalNameDialogMode.Rename )
        {
            await RunningTotalRenamed.InvokeAsync( (pendingRunningTotalRenameSourceName, confirmedName) );
            selectedRunningTotalName = confirmedName;
            pendingRunningTotalRenameSourceName = null;
        }
    }

    private async Task RunningTotalDialogConfirmed( ReportRunningTotalDefinition runningTotal )
    {
        if ( runningTotal is null )
            return;

        selectedRunningTotalName = runningTotal.Name;
        await RunningTotalConfirmed.InvokeAsync( runningTotal );
    }

    private async Task OpenFormulaFieldEditorAsync( string formulaFieldName )
    {
        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        ReportFormulaFieldDefinition formulaField = Definition?.FormulaFields?.FirstOrDefault( field =>
            string.Equals( field.Name, formulaFieldName, StringComparison.OrdinalIgnoreCase ) );

        pendingFormulaFieldName = formulaFieldName.Trim();
        selectedFormulaFieldName = pendingFormulaFieldName;

        await formulaDialogRef.ShowAsync( pendingFormulaFieldName, formulaField?.Formula );
    }

    private async Task NewFormulaFieldClicked( MouseEventArgs eventArgs )
    {
        await CloseFormulaFieldContextMenu();
        formulaFieldNameDialogMode = ReportFormulaFieldNameDialogMode.Create;
        await formulaFieldNameDialogRef.ShowAsync( CreateFormulaFieldName(), "New Formula Field" );
    }

    private async Task NewRunningTotalClicked( MouseEventArgs eventArgs )
    {
        await CloseRunningTotalContextMenu();
        await runningTotalDialogRef.ShowAsync( new()
        {
            Name = CreateRunningTotalName(),
            Function = ReportAggregateFunction.Sum,
        } );
    }

    private async Task EditFormulaFieldClicked( MouseEventArgs eventArgs )
    {
        string formulaFieldName = formulaFieldContextMenu?.FormulaFieldName;

        await CloseFormulaFieldContextMenu();
        await OpenFormulaFieldEditorAsync( formulaFieldName );
    }

    private async Task RenameFormulaFieldClicked( MouseEventArgs eventArgs )
    {
        string formulaFieldName = formulaFieldContextMenu?.FormulaFieldName;

        if ( string.IsNullOrWhiteSpace( formulaFieldName ) )
            return;

        await CloseFormulaFieldContextMenu();
        formulaFieldNameDialogMode = ReportFormulaFieldNameDialogMode.Rename;
        pendingFormulaFieldRenameSourceName = formulaFieldName;
        await formulaFieldNameDialogRef.ShowAsync( formulaFieldName, "Rename Formula Field" );
    }

    private async Task DeleteFormulaFieldClicked( MouseEventArgs eventArgs )
    {
        string formulaFieldName = formulaFieldContextMenu?.FormulaFieldName;

        await CloseFormulaFieldContextMenu();

        if ( !string.IsNullOrWhiteSpace( formulaFieldName ) )
        {
            if ( string.Equals( selectedFormulaFieldName, formulaFieldName, StringComparison.OrdinalIgnoreCase ) )
                selectedFormulaFieldName = null;

            await FormulaFieldDeleted.InvokeAsync( formulaFieldName );
        }
    }

    private async Task EditRunningTotalClicked( MouseEventArgs eventArgs )
    {
        string runningTotalName = runningTotalContextMenu?.RunningTotalName;

        await CloseRunningTotalContextMenu();

        ReportRunningTotalDefinition runningTotal = FindRunningTotal( runningTotalName );

        if ( runningTotal is not null )
            await runningTotalDialogRef.ShowAsync( runningTotal );
    }

    private async Task RenameRunningTotalClicked( MouseEventArgs eventArgs )
    {
        string runningTotalName = runningTotalContextMenu?.RunningTotalName;

        if ( string.IsNullOrWhiteSpace( runningTotalName ) )
            return;

        await CloseRunningTotalContextMenu();
        runningTotalNameDialogMode = ReportRunningTotalNameDialogMode.Rename;
        pendingRunningTotalRenameSourceName = runningTotalName;
        await runningTotalNameDialogRef.ShowAsync( runningTotalName, "Rename Running Total" );
    }

    private async Task DeleteRunningTotalClicked( MouseEventArgs eventArgs )
    {
        string runningTotalName = runningTotalContextMenu?.RunningTotalName;

        await CloseRunningTotalContextMenu();

        if ( !string.IsNullOrWhiteSpace( runningTotalName ) )
        {
            if ( string.Equals( selectedRunningTotalName, runningTotalName, StringComparison.OrdinalIgnoreCase ) )
                selectedRunningTotalName = null;

            await RunningTotalDeleted.InvokeAsync( runningTotalName );
        }
    }

    private async Task RefreshDataSourceClicked( MouseEventArgs eventArgs )
    {
        string dataSourceName = dataSourceContextMenu?.DataSourceName;

        await CloseDataSourceContextMenu();

        if ( !string.IsNullOrWhiteSpace( dataSourceName ) )
            await DataSourceRefreshed.InvokeAsync( dataSourceName );
    }

    private async Task DeleteDataSourceClicked( MouseEventArgs eventArgs )
    {
        string dataSourceName = dataSourceContextMenu?.DataSourceName;

        await CloseDataSourceContextMenu();

        if ( !string.IsNullOrWhiteSpace( dataSourceName ) )
            await DataSourceDeleted.InvokeAsync( dataSourceName );
    }

    private Task CloseFormulaFieldContextMenu()
    {
        formulaFieldContextMenu = null;

        return Task.CompletedTask;
    }

    private Task CloseDataSourceContextMenu()
    {
        dataSourceContextMenu = null;

        return Task.CompletedTask;
    }

    private Task CloseRunningTotalContextMenu()
    {
        runningTotalContextMenu = null;

        return Task.CompletedTask;
    }

    private string CreateFormulaFieldName()
    {
        const string baseName = "Formula";

        int index = 1;
        string name = $"{baseName}{index}";

        while ( Definition?.FormulaFields?.Any( field => string.Equals( field.Name, name, StringComparison.OrdinalIgnoreCase ) ) == true )
        {
            index++;
            name = $"{baseName}{index}";
        }

        return name;
    }

    private string CreateRunningTotalName()
    {
        const string baseName = "RunningTotal";

        int index = 1;
        string name = $"{baseName}{index}";

        while ( Definition?.RunningTotals?.Any( field => string.Equals( field.Name, name, StringComparison.OrdinalIgnoreCase ) ) == true )
        {
            index++;
            name = $"{baseName}{index}";
        }

        return name;
    }

    private ReportRunningTotalDefinition FindRunningTotal( string runningTotalName )
    {
        return Definition?.RunningTotals?.FirstOrDefault( field =>
            string.Equals( field.Name, runningTotalName, StringComparison.OrdinalIgnoreCase ) );
    }

    #endregion

    #region Properties

    private IReadOnlyList<ReportDesignerDataSourceNode> DataSources
        => ReportDataSourceExplorer.ResolveDataSourceDictionary( Definition, DataSourceName ).ToList();

    private bool FormulaFieldContextMenuVisible => formulaFieldContextMenu?.Visible == true;

    private bool RunningTotalContextMenuVisible => runningTotalContextMenu?.Visible == true;

    private bool DataSourceContextMenuVisible => dataSourceContextMenu?.Visible == true;

    private bool IsFormulaFieldContextMenuTarget => !string.IsNullOrWhiteSpace( formulaFieldContextMenu?.FormulaFieldName );

    private bool IsRunningTotalContextMenuTarget => !string.IsNullOrWhiteSpace( runningTotalContextMenu?.RunningTotalName );

    private string NewFormulaFieldContextMenuText => IsFormulaFieldContextMenuTarget ? "New" : "New formula field";

    private string NewRunningTotalContextMenuText => IsRunningTotalContextMenuTarget ? "New" : "New running total";

    /// <summary>
    /// Report definition used to discover data source fields.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Report data used to validate formula fields.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Default report data source name shown in the field tree.
    /// </summary>
    [Parameter] public string DataSourceName { get; set; }

    /// <summary>
    /// Raised when a field tree item starts dragging.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeDragEventArgs> FieldsNodeDragStarted { get; set; }

    /// <summary>
    /// Raised when a tree node drag operation ends.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNode> NodeDragEnded { get; set; }

    /// <summary>
    /// Raised when a formula field is created or updated.
    /// </summary>
    [Parameter] public EventCallback<ReportFormulaFieldDefinition> FormulaFieldConfirmed { get; set; }

    /// <summary>
    /// Raised when a formula field is renamed.
    /// </summary>
    [Parameter] public EventCallback<(string OldName, string NewName)> FormulaFieldRenamed { get; set; }

    /// <summary>
    /// Raised when a formula field is deleted.
    /// </summary>
    [Parameter] public EventCallback<string> FormulaFieldDeleted { get; set; }

    /// <summary>
    /// Raised when a formula field should be inserted into the selected band.
    /// </summary>
    [Parameter] public EventCallback<string> FormulaFieldInserted { get; set; }

    /// <summary>
    /// Raised when a source or special field should be inserted into the selected band.
    /// </summary>
    [Parameter] public EventCallback<(string DataSourceName, string FieldName)> FieldInserted { get; set; }

    /// <summary>
    /// Raised when a running total field is created or updated.
    /// </summary>
    [Parameter] public EventCallback<ReportRunningTotalDefinition> RunningTotalConfirmed { get; set; }

    /// <summary>
    /// Raised when a running total field is renamed.
    /// </summary>
    [Parameter] public EventCallback<(string OldName, string NewName)> RunningTotalRenamed { get; set; }

    /// <summary>
    /// Raised when a running total field is deleted.
    /// </summary>
    [Parameter] public EventCallback<string> RunningTotalDeleted { get; set; }

    /// <summary>
    /// Raised when a running total field should be inserted into the selected band.
    /// </summary>
    [Parameter] public EventCallback<string> RunningTotalInserted { get; set; }

    /// <summary>
    /// Raised when a data source should be refreshed.
    /// </summary>
    [Parameter] public EventCallback<string> DataSourceRefreshed { get; set; }

    /// <summary>
    /// Raised when a data source should be deleted.
    /// </summary>
    [Parameter] public EventCallback<string> DataSourceDeleted { get; set; }

    #endregion

    #region Enums

    private enum ReportFormulaFieldNameDialogMode
    {
        Create,

        Rename,
    }

    private enum ReportRunningTotalNameDialogMode
    {
        Rename,
    }

    #endregion

    #region Classes

    private sealed class ReportFormulaFieldContextMenuState
    {
        internal bool Visible { get; set; }

        internal string FormulaFieldName { get; set; }

        internal double ClientX { get; set; }

        internal double ClientY { get; set; }
    }

    private sealed class ReportRunningTotalContextMenuState
    {
        internal bool Visible { get; set; }

        internal string RunningTotalName { get; set; }

        internal double ClientX { get; set; }

        internal double ClientY { get; set; }
    }

    private sealed class ReportDataSourceContextMenuState
    {
        internal bool Visible { get; set; }

        internal string DataSourceName { get; set; }

        internal double ClientX { get; set; }

        internal double ClientY { get; set; }
    }

    #endregion
}