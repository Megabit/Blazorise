#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation for the utilities JS module.
/// </summary>
public class JSUtilitiesModule : BaseJSModule, IJSUtilitiesModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSUtilitiesModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask AddClass( ElementReference elementRef, string classname )
        => InvokeSafeVoidAsync( "addClass", elementRef, classname );

    /// <inheritdoc/>
    public virtual ValueTask RemoveClass( ElementReference elementRef, string classname )
        => InvokeSafeVoidAsync( "removeClass", elementRef, classname );

    /// <inheritdoc/>
    public virtual ValueTask ToggleClass( ElementReference elementRef, string classname )
        => InvokeSafeVoidAsync( "toggleClass", elementRef, classname );

    /// <inheritdoc/>
    public virtual ValueTask AddClassToBody( string classname )
        => InvokeSafeVoidAsync( "addClassToBody", classname );

    /// <inheritdoc/>
    public virtual ValueTask RemoveClassFromBody( string classname )
        => InvokeSafeVoidAsync( "removeClassFromBody", classname );

    /// <inheritdoc/>
    public virtual ValueTask<bool> ParentHasClass( ElementReference elementRef, string classname )
        => InvokeSafeAsync<bool>( "parentHasClass", elementRef, classname );

    /// <inheritdoc/>
    public virtual ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement )
        => InvokeSafeVoidAsync( "focus", ResolveElementReference( elementRef ), elementId, scrollToElement );

    /// <inheritdoc/>
    public virtual ValueTask Select( ElementReference elementRef, string elementId, bool focus )
        => InvokeSafeVoidAsync( "select", ResolveElementReference( elementRef ), elementId, focus );

    /// <inheritdoc/>
    public virtual ValueTask ShowPicker( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "showPicker", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask ScrollAnchorIntoView( string anchorTarget )
        => InvokeSafeVoidAsync( "scrollAnchorIntoView", anchorTarget );

    /// <inheritdoc/>
    public virtual ValueTask ScrollElementIntoView( string elementId, bool smooth = true )
        => InvokeSafeVoidAsync( "scrollElementIntoView", elementId, smooth );

    /// <inheritdoc/>
    public virtual ValueTask SetCaret( ElementReference elementRef, int caret )
        => InvokeSafeVoidAsync( "setCaret", elementRef, caret );

    /// <inheritdoc/>
    public virtual ValueTask<int> GetCaret( ElementReference elementRef )
        => InvokeSafeAsync<int>( "getCaret", elementRef );

    /// <inheritdoc/>
    public virtual ValueTask SetTextValue( ElementReference elementRef, object value )
        => InvokeSafeVoidAsync( "setTextValue", elementRef, value );

    /// <inheritdoc/>
    public virtual ValueTask SetProperty( ElementReference elementRef, string property, object value )
        => InvokeSafeVoidAsync( "setProperty", elementRef, property, value );

    /// <inheritdoc/>
    public virtual ValueTask<DomElement> GetElementInfo( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<DomElement>( "getElementInfo", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask<string> GetUserAgent()
        => InvokeSafeAsync<string>( "getUserAgent" );

    /// <inheritdoc/>
    public ValueTask CopyToClipboard( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "copyToClipboard", elementRef, elementId );

    /// <inheritdoc/>
    public ValueTask Log( string message, params string[] args )
        => InvokeSafeVoidAsync( "log", message, args );

    private ElementReference? ResolveElementReference( ElementReference elementReference )
        => elementReference.Context is null ? null : elementReference;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/utilities.js?v={VersionProvider.Version}";

    #endregion
}