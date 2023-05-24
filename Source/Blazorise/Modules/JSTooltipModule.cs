﻿#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the tooltip JS module.
/// </summary>
public abstract class JSTooltipModule : BaseJSModule, IJSTooltipModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "initialize", elementRef, elementId, options );

    /// <inheritdoc/>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateContent( ElementReference elementRef, string elementId, string content )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "updateContent", elementRef, elementId, content );
    }

    #endregion
}