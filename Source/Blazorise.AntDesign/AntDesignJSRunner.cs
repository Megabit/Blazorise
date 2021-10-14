#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        public override ValueTask InitializeTooltip( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( $"{ANTDESIGN_NAMESPACE}.tooltip.initialize", elementRef, elementId, options );
        }
    }
}
