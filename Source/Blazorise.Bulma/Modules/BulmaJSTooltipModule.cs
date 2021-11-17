﻿using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bulma.Modules
{
    internal class BulmaJSTooltipModule : JSTooltipModule
    {
        public BulmaJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.Bulma/tooltip.js?v={VersionProvider.Version}";
    }
}
