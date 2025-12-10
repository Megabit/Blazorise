#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Splitter;

/// <summary>
/// Default implementation of the splitter JS module.
/// </summary>
public class JSSplitModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Constructor

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSSplitModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options ) : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes a new splitter instance.
    /// </summary>
    /// <param name="elementId">Id of the splitter element.</param>
    /// <param name="elementRef">Reference to the splitter element.</param>
    /// <param name="sectionElementRefs">Sections of the splitter.</param>
    /// <param name="splitterOptions">Splitter configuration options.</param>
    /// <param name="splitterGutterOptions">Gutter configuration options</param>
    /// <returns>A <see cref="IJSObjectReference"/> to the Splitter object.</returns>
    public virtual ValueTask InitializeSplitter( ElementReference elementRef, string elementId, IEnumerable<ElementReference> sectionElementRefs, SplitterOptions splitterOptions, SplitterGutterOptions splitterGutterOptions )
    {
        return InvokeSafeVoidAsync( "initializeSplitter", elementRef, elementId, sectionElementRefs, splitterOptions, splitterGutterOptions );
    }

    public virtual ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        return InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public override string ModuleFileName => $"./_content/Blazorise.Splitter/blazorise.splitter.js?v={VersionProvider.Version}";

    #endregion
}