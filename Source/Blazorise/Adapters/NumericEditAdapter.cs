#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Middleman between the NumericEdit component and javascript.
    /// </summary>
    public class NumericEditAdapter
    {
        private readonly INumericEdit numericEdit;

        public NumericEditAdapter( INumericEdit numericEdit )
        {
            this.numericEdit = numericEdit;
        }

        [JSInvokable]
        public Task SetValue( string value )
        {
            return numericEdit.SetValue( value );
        }
    }
}
