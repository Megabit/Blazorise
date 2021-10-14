#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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
