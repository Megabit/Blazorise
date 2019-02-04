#region Using directives
using Microsoft.JSInterop;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Material
{
    public partial class MaterialJSRunner : Blazorise.JSRunner
    {
        public override Task<bool> ActivateDatePicker( string elementId, string formatSubmit )
        {
            return JSRuntime.Current.InvokeAsync<bool>( $"blazoriseMaterial.activateDatePicker", elementId, formatSubmit );
        }
    }
}
