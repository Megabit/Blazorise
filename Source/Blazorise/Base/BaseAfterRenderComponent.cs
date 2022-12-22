#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base render component that implements render-queue logic.
/// </summary>
public abstract class BaseAfterRenderComponent : ComponentBase
{
    #region Members

    /// <summary>
    /// A stack of functions to execute after the rendering.
    /// </summary>
    private Queue<Func<Task>> executeAfterRenderQueue;

    /// <summary>
    /// A stack of functions to execute after the rendering. Used only during the component initialization.
    /// </summary>
    private Queue<Func<Task>> delayedExecuteAfterRenderQueue;

    #endregion

    #region Methods

    /// <summary>
    /// Pushes an action to the stack to be executed after the rendering is done.
    /// </summary>
    /// <param name="action"></param>
    protected void ExecuteAfterRender( Func<Task> action )
    {
        if ( !Rendered )
        {
            delayedExecuteAfterRenderQueue ??= new();
            delayedExecuteAfterRenderQueue.Enqueue( action );
        }
        else
        {
            executeAfterRenderQueue ??= new();
            executeAfterRenderQueue.Enqueue( action );
        }
    }

    private bool PushDelayedExecuteAfterRender()
    {
        if ( delayedExecuteAfterRenderQueue?.Count > 0 )
        {
            while ( delayedExecuteAfterRenderQueue.Count > 0 )
            {
                var action = delayedExecuteAfterRenderQueue.Dequeue();

                ExecuteAfterRender( action );
            }

            delayedExecuteAfterRenderQueue = null;

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await OnFirstAfterRenderAsync();

            Rendered = true;

            if ( PushDelayedExecuteAfterRender() )
            {
                await InvokeAsync( StateHasChanged );
            }
        }

        Rendered = true;

        if ( executeAfterRenderQueue?.Count > 0 )
        {
            while ( executeAfterRenderQueue.Count > 0 )
            {
                var action = executeAfterRenderQueue.Dequeue();

                await action();
            }
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Method is called only once when component is first rendered.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task OnFirstAfterRenderAsync()
        => Task.CompletedTask;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose( true );
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="BaseComponent"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True if the component is in the disposing process.</param>
    protected virtual void Dispose( bool disposing )
    {
        if ( !Disposed )
        {
            Disposed = true;

            if ( disposing )
            {
                executeAfterRenderQueue?.Clear();

                ComponentDisposer.Dispose( this );
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync( true );

        Dispose( false );
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="BaseComponent"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True if the component is in the disposing process.</param>
    protected virtual ValueTask DisposeAsync( bool disposing )
    {
        try
        {
            if ( !AsyncDisposed )
            {
                AsyncDisposed = true;

                if ( disposing )
                {
                    executeAfterRenderQueue?.Clear();

                    ComponentDisposer.Dispose( this );
                }
            }

            return default;
        }
        catch ( Exception exc )
        {
            return new( Task.FromException( exc ) );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the component is already fully disposed.
    /// </summary>
    protected internal bool Disposed { get; private set; }

    /// <summary>
    /// Indicates if the component is already fully disposed (asynchronously).
    /// </summary>
    protected internal bool AsyncDisposed { get; private set; }

    /// <summary>
    /// Indicates if component has been rendered in the browser.
    /// </summary>
    protected bool Rendered { get; private set; }

    /// <summary>
    /// Service that frees the component from the memory.
    /// </summary>
    [Inject] protected internal IComponentDisposer ComponentDisposer { get; set; }

    #endregion
}