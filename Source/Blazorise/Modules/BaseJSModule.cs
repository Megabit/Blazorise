#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <inheritdoc/>
public abstract class BaseJSModule : IBaseJSModule, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// JavaScript runtime instance.
    /// </summary>
    private readonly IJSRuntime jsRuntime;

    /// <summary>
    /// Version provider instance.
    /// </summary>
    private readonly IVersionProvider versionProvider;

    /// <summary>
    /// Awaitable module instance.
    /// </summary>
    private Task<IJSObjectReference> moduleTask;

    #endregion

    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public BaseJSModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
    {
        this.jsRuntime = jsRuntime;
        this.versionProvider = versionProvider;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invokes the module instance javascript function.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    protected async ValueTask InvokeVoidAsync( string identifier, params object[] args )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( identifier, args );
    }

    /// <summary>
    /// Invokes the module instance javascript function.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    protected async ValueTask<TValue> InvokeAsync<TValue>( string identifier, params object[] args )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<TValue>( identifier, args );
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync( true );
    }

    /// <summary>
    /// Releases the unmanaged resources used by the module and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True if the component is in the disposing process.</param>
    protected virtual async ValueTask DisposeAsync( bool disposing )
    {
        try
        {
            if ( !AsyncDisposed )
            {
                AsyncDisposed = true;

                if ( disposing )
                {
                    if ( moduleTask is not null )
                    {
                        var moduleInstance = await moduleTask;
                        await moduleInstance.SafeDisposeAsync();

                        moduleTask = null;
                    }
                }
            }
        }
        catch ( Exception exc )
        {
            await Task.FromException( exc );
        }
    }

    /// <summary>
    /// Save invocation on the JavaScript <see cref="Module"/>.
    /// </summary>
    /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>someScope.someFunction</c> on the target instance.</param>
    /// <param name="args">JSON-serializable arguments.</param>
    protected async ValueTask InvokeSafeVoidAsync( string identifier, params object[] args )
    {
        try
        {
            var module = await Module;

            if ( AsyncDisposed )
            {
                return;
            }

            await module.InvokeVoidAsync( identifier, args );
        }
        catch ( Exception exc ) when ( exc is JSDisconnectedException or ObjectDisposedException or TaskCanceledException )
        {
        }
    }

    /// <summary>
    /// Save invocation on the JavaScript <see cref="Module"/>.
    /// </summary>
    /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
    /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>someScope.someFunction</c> on the target instance.</param>
    /// <param name="args">JSON-serializable arguments.</param>
    /// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-deserializing the return value.</returns>
    protected async ValueTask<TValue> InvokeSafeAsync<TValue>( string identifier, params object[] args )
    {
        try
        {
            var module = await Module;

            if ( AsyncDisposed )
            {
                return default;
            }

            return await module.InvokeAsync<TValue>( identifier, args );
        }
        catch ( Exception exc ) when ( exc is JSDisconnectedException or ObjectDisposedException or TaskCanceledException )
        {
            return default;
        }
    }

    private Task<IJSObjectReference> GetModule()
    {
        return moduleTask ??= jsRuntime.InvokeAsync<IJSObjectReference>( "import", ModuleFileName ).AsTask();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns true if module was already being destroyed.
    /// </summary>
    protected bool IsUnsafe => AsyncDisposed || moduleTask == null;

    /// <summary>
    /// Indicates if the component is already fully disposed (asynchronously).
    /// </summary>
    protected bool AsyncDisposed { get; private set; }

    /// <inheritdoc/>
    public Task<IJSObjectReference> Module => GetModule();

    /// <inheritdoc/>
    public abstract string ModuleFileName { get; }

    /// <summary>
    /// Gets the JavaScript runtime instance.
    /// </summary>
    public IJSRuntime JSRuntime => jsRuntime;

    /// <summary>
    /// Gets the version provider instance.
    /// </summary>
    protected IVersionProvider VersionProvider => versionProvider;

    #endregion
}