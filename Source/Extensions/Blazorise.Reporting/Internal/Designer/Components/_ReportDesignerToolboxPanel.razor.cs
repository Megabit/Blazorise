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
/// Renders the report designer toolbox and fields explorer.
/// </summary>
public partial class _ReportDesignerToolboxPanel
{
    #region Members

    private const string FieldsExplorerTab = "FieldsExplorer";

    private const string ToolboxTab = "Toolbox";

    private string selectedTab = FieldsExplorerTab;

    private string pendingFormulaFieldName;

    private string pendingFormulaFieldRenameSourceName;

    private string selectedFormulaFieldName;

    private ReportFormulaFieldNameDialogMode formulaFieldNameDialogMode;

    private ReportFormulaFieldContextMenuState formulaFieldContextMenu;

    private _ReportDesignerFormulaDialog formulaDialogRef;

    private _ReportDesignerFormulaFieldNameDialog formulaFieldNameDialogRef;

    #endregion

    #region Methods

    private Color GetTabColor( string tab )
    {
        return string.Equals( selectedTab, tab, StringComparison.Ordinal ) ? Color.Primary : Color.Light;
    }

    private Task SelectTab( string tab )
    {
        selectedTab = tab;

        return Task.CompletedTask;
    }

    private async Task FieldsNodeClicked( ReportTreeNode node )
    {
        if ( node?.Kind == ReportTreeNodeKind.FormulaField )
            selectedFormulaFieldName = node.Text;

        await CloseFormulaFieldContextMenu();
    }

    private async Task FieldsNodeDoubleClicked( ReportTreeNode node )
    {
        if ( node?.Kind != ReportTreeNodeKind.FormulaField )
            return;

        selectedFormulaFieldName = node.Text;
        await CloseFormulaFieldContextMenu();
        await FormulaFieldInserted.InvokeAsync( node.Text );
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

    private Task CloseFormulaFieldContextMenu()
    {
        formulaFieldContextMenu = null;

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

    #endregion

    #region Properties

    private bool IsToolboxSelected => string.Equals( selectedTab, ToolboxTab, StringComparison.Ordinal );

    private IReadOnlyList<ReportDesignerDataSourceNode> DataSources
        => ReportDataSourceExplorer.ResolveDataSourceDictionary( Definition, DataSourceName ).ToList();

    private bool FormulaFieldContextMenuVisible => formulaFieldContextMenu?.Visible == true;

    private bool IsFormulaFieldContextMenuTarget => !string.IsNullOrWhiteSpace( formulaFieldContextMenu?.FormulaFieldName );

    private string NewFormulaFieldContextMenuText => IsFormulaFieldContextMenuTarget ? "New" : "New formula field";

    /// <summary>
    /// Report definition used to discover toolbox and data source fields.
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
    /// Raised when a toolbox item starts dragging.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeDragEventArgs> ToolboxNodeDragStarted { get; set; }

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

    #endregion

    #region Enums

    private enum ReportFormulaFieldNameDialogMode
    {
        Create,

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

    #endregion
}