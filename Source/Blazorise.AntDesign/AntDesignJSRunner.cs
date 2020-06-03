#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public partial class AntDesignJSRunner : JSRunner
    {
        private const string ANTDESIGN_NAMESPACE = "antDesign";

        public AntDesignJSRunner( IJSRuntime runtime )
            : base( runtime )
        {
        }

        public override ValueTask<bool> InitializeTooltip( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{ANTDESIGN_NAMESPACE}.tooltip.initialize", elementRef, elementId );
        }

        public override ValueTask<bool> OpenModal( ElementReference elementRef, string elementId, bool scrollToTop )
        {
            return runtime.InvokeAsync<bool>( $"{ANTDESIGN_NAMESPACE}.modal.open", elementRef, elementId, scrollToTop );
        }

        public override ValueTask<bool> CloseModal( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{ANTDESIGN_NAMESPACE}.modal.close", elementRef, elementId );
        }
    }
}
