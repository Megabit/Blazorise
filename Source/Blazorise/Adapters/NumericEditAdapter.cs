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

        /// <summary>
        /// Default constructor for <see cref="NumericEditAdapter"/>.
        /// </summary>
        /// <param name="numericEdit">Numeric input to which the adapter is referenced.</param>
        public NumericEditAdapter( INumericEdit numericEdit )
        {
            this.numericEdit = numericEdit;
        }

        /// <summary>
        /// Notify us from JS that input value changed.
        /// </summary>
        /// <param name="value">New input value.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [JSInvokable]
        public Task SetValue( string value )
        {
            return numericEdit.SetValue( value );
        }
    }
}
