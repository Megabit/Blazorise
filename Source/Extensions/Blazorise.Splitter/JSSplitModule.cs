using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Splitter;

/// <summary>
/// Default implementation of the split JS module.
/// </summary>
public class JSSplitModule : BaseJSModule
{
    #region Constructor

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSSplitModule( IJSRuntime jsRuntime, IVersionProvider versionProvider ) : base( jsRuntime, versionProvider )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes a new Split instance.
    /// </summary>
    /// <param name="sectionElementRefs">Sections of the splitter.</param>
    /// <param name="splitterOptions">Splitter configuration options.</param>
    /// <param name="splitterGutterOptions">Gutter configuration options</param>
    /// <returns>A <see cref="IJSObjectReference"/> to the Split object.</returns>
    public virtual ValueTask<IJSObjectReference> InitializeSplitter( IEnumerable<ElementReference> sectionElementRefs, SplitterOptions splitterOptions, SplitterGutterOptions splitterGutterOptions )
        => InvokeSafeAsync<IJSObjectReference>( "initializeSplitter", sectionElementRefs, splitterOptions, splitterGutterOptions );

    #endregion

    #region Properties

    /// <inheritdoc />
    public override string ModuleFileName => $"./_content/Blazorise.Splitter/blazorise.splitter.js?v={VersionProvider.Version}";

    #endregion
}