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

        public override ValueTask OpenModal( ElementReference elementRef, bool scrollToTop )
        {
            throw new NotImplementedException();
        }

        public override ValueTask CloseModal( ElementReference elementRef )
        {
            throw new NotImplementedException();
        }
    }
}
