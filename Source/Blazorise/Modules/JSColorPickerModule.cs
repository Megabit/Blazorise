﻿#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="ColorPicker"/> JS module.
/// </summary>
public class JSColorPickerModule : BaseJSModule, IJSColorPickerModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSColorPickerModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( DotNetObjectReference<ColorPicker> dotNetObjectRef, ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId, options );

    /// <inheritdoc/>
    public virtual ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask Activate( ElementReference elementRef, string elementId, object options )
        => ValueTask.CompletedTask;

    /// <inheritdoc/>
    public virtual ValueTask UpdateValue( ElementReference elementRef, string elementId, object value )
        => InvokeSafeVoidAsync( "updateValue", elementRef, elementId, value );

    /// <inheritdoc/>
    public virtual ValueTask UpdateOptions( ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "updateOptions", elementRef, elementId, options );

    /// <inheritdoc/>
    public virtual ValueTask UpdateLocalization( ElementReference elementRef, string elementId, object localization )
        => InvokeSafeVoidAsync( "updateLocalization", elementRef, elementId, localization );

    /// <inheritdoc/>
    public virtual ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement )
        => InvokeSafeVoidAsync( "focus", elementRef, elementId, scrollToElement );

    /// <inheritdoc/>
    public virtual ValueTask Select( ElementReference elementRef, string elementId, bool focus )
        => InvokeSafeVoidAsync( "select", elementRef, elementId, focus );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/colorPicker.js?v={VersionProvider.Version}";

    #endregion
}