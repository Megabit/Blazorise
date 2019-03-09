#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Providers
{
    class EmptyJSRunner : JSRunner
    {
        public EmptyJSRunner( IJSRuntime runtime )
            : base( runtime )
        {

        }
    }
}
