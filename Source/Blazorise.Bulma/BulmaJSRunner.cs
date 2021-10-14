#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bulma
{
    public partial class BulmaJSRunner : Blazorise.JSRunner
    {
        public BulmaJSRunner( IJSRuntime runtime )
            : base( runtime )
        {

        }

        public override ValueTask InitializeTooltip( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( "blazoriseBulma.tooltip.initialize", elementRef, elementId, options );
        }
    }
}
