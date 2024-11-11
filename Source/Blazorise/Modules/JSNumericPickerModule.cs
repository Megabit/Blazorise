#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules.JSOptions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="NumericPicker{TValue}"/> JS module.
/// </summary>
public class JSNumericPickerModule : BaseJSModule, IJSNumericPickerModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSNumericPickerModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( DotNetObjectReference<NumericPickerAdapter> dotNetObjectRef, ElementReference elementRef, string elementId, NumericPickerInitializeJSOptions options )
        => InvokeSafeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId, options );

    /// <inheritdoc/>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateOptions( ElementReference elementRef, string elementId, NumericPickerUpdateJSOptions options )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "updateOptions", elementRef, elementId, options );
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateValue<TValue>( ElementReference elementRef, string elementId, TValue value )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "updateValue", elementRef, elementId, value );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/numericPicker.js?v={VersionProvider.Version}";

    #endregion
}