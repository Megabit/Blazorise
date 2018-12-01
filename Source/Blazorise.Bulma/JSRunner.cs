using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazorise.Bulma
{
    public partial class JSRunner : Blazorise.JSRunner
    {
        public override Task<bool> ActivateDatePicker( string elementId )
        {
            Console.WriteLine( "Bulma date not implemented." );
            return Task.FromResult( true );
        }
    }
}
