#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Shared responsive breakpoint service for the current Blazor circuit.
/// </summary>
public class BreakpointService : IBreakpointService, IBreakpointActivator
{
    #region Members

    private readonly IJSBreakpointModule jsBreakpointModule;

    private readonly string elementId;

    private DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef;

    private Task initializationTask;

    private Breakpoint current = Breakpoint.None;

    private bool isResolved;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for <see cref="BreakpointService"/>.
    /// </summary>
    /// <param name="idGenerator">Blazorise id generator.</param>
    /// <param name="jsBreakpointModule">Breakpoint JS module.</param>
    public BreakpointService( IIdGenerator idGenerator, IJSBreakpointModule jsBreakpointModule )
    {
        this.jsBreakpointModule = jsBreakpointModule;
        this.elementId = $"b-breakpoint-service-{idGenerator.Generate}";
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public Task EnsureInitializedAsync()
    {
        return initializationTask ??= InitializeAsync();
    }

    /// <inheritdoc/>
    public bool IsAtLeast( Breakpoint breakpoint )
    {
        return IsResolved && Current.IsAtLeast( breakpoint );
    }

    /// <inheritdoc/>
    public bool IsBelow( Breakpoint breakpoint )
    {
        return IsResolved && Current.IsBelow( breakpoint );
    }

    /// <inheritdoc/>
    public async Task OnBreakpoint( bool broken )
    {
        await RefreshCurrentBreakpointAsync( true );
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        try
        {
            await jsBreakpointModule.UnregisterBreakpoint( this );
        }
        finally
        {
            dotNetObjectRef?.Dispose();
            dotNetObjectRef = null;
            Changed = null;
        }
    }

    private async Task InitializeAsync()
    {
        dotNetObjectRef ??= DotNetObjectReference.Create( new BreakpointActivatorAdapter( this ) );

        await jsBreakpointModule.RegisterBreakpoint( dotNetObjectRef, ElementId );
        await RefreshCurrentBreakpointAsync( false );
    }

    private async Task RefreshCurrentBreakpointAsync( bool notify )
    {
        string breakpointName = await jsBreakpointModule.GetBreakpoint();
        Breakpoint nextBreakpoint = breakpointName.ParseBreakpoint();
        bool nextIsResolved = nextBreakpoint != Breakpoint.None;
        bool changed = current != nextBreakpoint || isResolved != nextIsResolved;

        current = nextBreakpoint;
        isResolved = nextIsResolved;

        if ( notify && changed && isResolved )
        {
            await NotifyChangedAsync( nextBreakpoint );
        }
    }

    private async Task NotifyChangedAsync( Breakpoint breakpoint )
    {
        Func<Breakpoint, Task> handlers = Changed;

        if ( handlers is null )
            return;

        foreach ( Func<Breakpoint, Task> handler in handlers.GetInvocationList() )
        {
            await handler( breakpoint );
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public event Func<Breakpoint, Task> Changed;

    /// <inheritdoc/>
    public Breakpoint Current => current;

    /// <inheritdoc/>
    public bool IsResolved => isResolved;

    /// <inheritdoc/>
    public string ElementId => elementId;

    /// <inheritdoc/>
    public Breakpoint Breakpoint => Breakpoint.None;

    #endregion
}