using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazorise.Material
{
    public partial class JSRunner : Blazorise.JSRunner
    {
        public override Task<bool> ActivateDatePicker( string elementId )
        {
            return JSRuntime.Current.InvokeAsync<bool>( $"blazoriseMaterial.activateDatePicker", elementId );
        }
    }
}
