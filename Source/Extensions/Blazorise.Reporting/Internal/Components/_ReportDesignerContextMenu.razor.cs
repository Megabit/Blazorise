#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders and controls the report designer context menu.
/// </summary>
public partial class _ReportDesignerContextMenu
{
    #region Members

    private ContextMenu contextMenuRef;

    #endregion

    #region Methods

    /// <summary>
    /// Shows the context menu with the supplied state.
    /// </summary>
    /// <param name="state">Context menu state to render.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal async Task Show( ReportContextMenuState state )
    {
        State = state;

        await InvokeAsync( StateHasChanged );

        if ( contextMenuRef is not null )
            await contextMenuRef.Show( state.ClientX, state.ClientY );
    }

    /// <summary>
    /// Hides the context menu.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal async Task CloseMenu()
    {
        if ( contextMenuRef is not null )
            await contextMenuRef.Hide();

        State = null;

        await InvokeAsync( StateHasChanged );
    }

    private Task InvokeCommand( object command )
    {
        if ( Command is not null )
            return Command.Invoke( command );

        return Task.CompletedTask;
    }

    private Task OnCommandClicked( object value )
    {
        return InvokeCommand( value );
    }

    #endregion

    #region Properties

    private bool IsPageMenu => State?.Target == ReportContextMenuTarget.Page;

    private bool IsSectionMenu => State?.Target == ReportContextMenuTarget.Section;

    private string SectionTypeDisplayName => State is null
        ? null
        : ReportDefinitionHelper.GetSectionTypeDisplayName( State.SectionType );

    private bool IsElementMenu => State?.Target == ReportContextMenuTarget.Element;

    private bool HasElementLayoutCommands => State?.CanOrderSelectedElements == true || State?.CanAlignOrSizeSelectedElements == true;

    private bool HasElementContentCommands => State?.CanInsertAggregate == true
        || State?.CanEditText == true
        || State?.CanEditFormula == true
        || State?.CanEditRunningTotal == true;

    private bool IsCellMenu => State?.Target == ReportContextMenuTarget.Cell;

    /// <summary>
    /// Current context menu state.
    /// </summary>
    internal ReportContextMenuState State { get; private set; }

    /// <summary>
    /// Raised when a context menu command is selected.
    /// </summary>
    [Parameter] public Func<object, Task> Command { get; set; }

    #endregion
}