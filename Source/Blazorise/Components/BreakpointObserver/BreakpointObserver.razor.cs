#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Observes the shared breakpoint service and re-renders its content when the responsive breakpoint changes.
/// </summary>
public partial class BreakpointObserver : BaseAfterRenderComponent, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        BreakpointService.Changed += HandleBreakpointChanged;
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        await BreakpointService.EnsureInitializedAsync();

        CurrentBreakpoint = BreakpointService.Current;

        if ( BreakpointService.IsResolved && BreakpointChanged.HasDelegate )
        {
            await BreakpointChanged.InvokeAsync( CurrentBreakpoint );
        }

        await InvokeAsync( StateHasChanged );
        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing && BreakpointService is not null )
        {
            BreakpointService.Changed -= HandleBreakpointChanged;
        }

        base.Dispose( disposing );
    }

    private async Task HandleBreakpointChanged( Breakpoint breakpoint )
    {
        CurrentBreakpoint = breakpoint;

        if ( BreakpointChanged.HasDelegate )
        {
            await BreakpointChanged.InvokeAsync( breakpoint );
        }

        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current responsive breakpoint.
    /// </summary>
    protected Breakpoint CurrentBreakpoint { get; set; }

    /// <summary>
    /// Gets or sets the shared breakpoint service.
    /// </summary>
    [Inject] protected IBreakpointService BreakpointService { get; set; }

    /// <summary>
    /// Raised whenever the observed responsive breakpoint changes.
    /// </summary>
    [Parameter] public EventCallback<Breakpoint> BreakpointChanged { get; set; }

    /// <summary>
    /// Content rendered with the current breakpoint as the template context.
    /// </summary>
    [Parameter] public RenderFragment<Breakpoint> ChildContent { get; set; }

    #endregion
}