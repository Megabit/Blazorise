﻿using System.Collections.Generic;
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
    /// Initializes a new Split instance
    /// </summary>
    /// <param name="sections">Sections of the splitter</param>
    /// <param name="options">Splitter configuration options</param>
    /// <returns>A <see cref="IJSObjectReference"/> to the Animation object</returns>
    public virtual ValueTask<IJSObjectReference> InitializeSplit( IEnumerable<ElementReference> sections, SplitterConfiguration options )
        => InvokeSafeAsync<IJSObjectReference>( "initializeSplit", sections, options );

    #endregion

    #region Properties

    /// <inheritdoc />
    public override string ModuleFileName => $"./_content/Blazorise.Splitter/blazorise.splitter.js?v={VersionProvider.Version}";

    #endregion

}