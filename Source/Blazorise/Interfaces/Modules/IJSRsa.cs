#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules
{
    internal interface IJSRsa : IBaseJSModule
    {
        ValueTask<bool> Verify( string content, string signature );
    }
}
