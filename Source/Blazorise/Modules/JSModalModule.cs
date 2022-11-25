﻿#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the modal JS module.
/// </summary>
public abstract class JSModalModule : BaseJSModule, IJSModalModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask OpenModal( ElementReference elementRef, bool scrollToTop )
        => InvokeSafeVoidAsync( "open", elementRef, scrollToTop );

    /// <inheritdoc/>
    public virtual async ValueTask CloseModal( ElementReference elementRef )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "close", elementRef );
    }

    #endregion
}