#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Material
{
    public partial class MaterialJSRunner : Blazorise.JSRunner
    {
        public MaterialJSRunner( IJSRuntime runtime )
               : base( runtime )
        {
        }

        public override ValueTask<bool> InitializeTooltip( string elementId, ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseMaterial.tooltip.initialize", elementId, elementRef );
        }

        public override ValueTask<bool> ActivateDatePicker( string elementId, string formatSubmit )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseMaterial.activateDatePicker", elementId, formatSubmit );
        }
    }
}
