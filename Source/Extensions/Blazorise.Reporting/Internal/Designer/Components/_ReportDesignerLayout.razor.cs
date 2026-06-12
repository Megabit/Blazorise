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

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the three-panel report designer layout.
/// </summary>
public partial class _ReportDesignerLayout
{
    #region Members

    private Div designerElement;

    private DotNetObjectReference<_ReportDesignerLayout> dotNetObjectReference;

    private JSReportingModule reportingModule;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && Shortcut.HasDelegate )
        {
            EnsureReportingModule();
            dotNetObjectReference ??= DotNetObjectReference.Create( this );

            await reportingModule.StartDesignerKeyboardShortcuts( designerElement.ElementRef, dotNetObjectReference );
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if ( reportingModule is not null )
        {
            try
            {
                if ( designerElement is not null )
                    await reportingModule.StopDesignerKeyboardShortcuts( designerElement.ElementRef );

                await reportingModule.DisposeAsync();
            }
            catch ( JSDisconnectedException )
            {
            }
        }

        dotNetObjectReference?.Dispose();
    }

    /// <summary>
    /// Receives a designer keyboard shortcut from the reporting JavaScript module.
    /// </summary>
    /// <param name="shortcut">Shortcut command name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task OnDesignerShortcut( string shortcut )
    {
        return Enum.TryParse<ReportDesignerShortcut>( shortcut, out var designerShortcut )
            ? InvokeAsync( () => Shortcut.InvokeAsync( designerShortcut ) )
            : Task.CompletedTask;
    }

    private void EnsureReportingModule()
    {
        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    #endregion

    #region Properties

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Content shown in the left designer toolbox and fields panel.
    /// </summary>
    [Parameter] public RenderFragment ToolboxPanel { get; set; }

    /// <summary>
    /// Content shown in the central designer surface.
    /// </summary>
    [Parameter] public RenderFragment Surface { get; set; }

    /// <summary>
    /// Content shown in the right designer properties and explorer panel.
    /// </summary>
    [Parameter] public RenderFragment Panel { get; set; }

    /// <summary>
    /// Floating context menu shown above the designer layout.
    /// </summary>
    [Parameter] public RenderFragment ContextMenu { get; set; }

    /// <summary>
    /// Raised when a standard designer keyboard shortcut is pressed.
    /// </summary>
    [Parameter] public EventCallback<ReportDesignerShortcut> Shortcut { get; set; }

    #endregion
}