#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a toolbar button that executes a report command.
/// </summary>
public partial class ReportToolbarItem
{
    [CascadingParameter] internal ReportToolbarContext ToolbarContext { get; set; }

    private Task OnClicked()
    {
        return ToolbarContext?.ExecuteAsync( Command ) ?? Task.CompletedTask;
    }

    private bool Disabled => ToolbarContext?.CanExecute( Command ) == false;

    private bool Active => ToolbarContext?.IsActive( Command ) == true;

    private Color ResolvedColor => Color ?? ( Active ? ActiveColor : InactiveColor );

    private string Text => string.IsNullOrWhiteSpace( Caption ) ? Command.ToString() : Caption;

    private IconName? ResolvedIcon => Icon ?? GetDefaultIcon( Command );

    private static IconName? GetDefaultIcon( ReportCommand command )
    {
        return command switch
        {
            ReportCommand.Design => IconName.Edit,
            ReportCommand.Preview => IconName.Eye,
            ReportCommand.PreviewHtml => IconName.FileAlt,
            ReportCommand.PreviewPdf => IconName.FilePdf,
            ReportCommand.ConnectDataSource => IconName.Database,
            ReportCommand.DownloadPdf => IconName.FileDownload,
            ReportCommand.Cut => IconName.Cut,
            ReportCommand.Copy => IconName.Copy,
            ReportCommand.Paste => IconName.Paste,
            ReportCommand.Delete => IconName.Delete,
            ReportCommand.Undo => IconName.Undo,
            ReportCommand.Redo => IconName.Redo,
            ReportCommand.Reset => IconName.Sync,
            _ => null,
        };
    }

    /// <summary>
    /// Report command executed when the toolbar item is clicked.
    /// </summary>
    [Parameter] public ReportCommand Command { get; set; }

    /// <summary>
    /// Text shown for the toolbar item instead of the command name.
    /// </summary>
    [Parameter] public string Caption { get; set; }

    /// <summary>
    /// Icon shown for the toolbar item instead of the default command icon.
    /// </summary>
    [Parameter] public IconName? Icon { get; set; }

    /// <summary>
    /// Shows the toolbar item caption next to the icon.
    /// </summary>
    [Parameter] public bool ShowCaption { get; set; }

    /// <summary>
    /// Explicit button color applied regardless of active state.
    /// </summary>
    [Parameter] public Color Color { get; set; }

    /// <summary>
    /// Button color used when the command represents the active report state.
    /// </summary>
    [Parameter] public Color ActiveColor { get; set; } = Color.Primary;

    /// <summary>
    /// Button color used when the command is available but not active.
    /// </summary>
    [Parameter] public Color InactiveColor { get; set; } = Color.Light;
}