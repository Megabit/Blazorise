#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class JSRunner : IJSRunner
    {
        #region Members

        private readonly IJSRuntime runtime;

        private const string BLAZORISE_NAMESPACE = "blazorise";

        #endregion

        #region Constructors

        public JSRunner( IJSRuntime runtime )
        {
            this.runtime = runtime;
        }

        #endregion

        #region Methods

        #region Select

        public async ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId )
        {
            // All of this is because Blazor is not serializing types as it should! In this case nullable types
            // are not working (enum?, int?, etc.) so we need to do it manually.

            // get the selected values for JS as strings
            var stringValues = await runtime.InvokeAsync<string[]>( $"{BLAZORISE_NAMESPACE}.getSelectedOptions", elementId );

            return stringValues?.Select( value =>
            {
                try
                {
                    if ( string.IsNullOrEmpty( value ) )
                        return default;

                    return Converters.ChangeType<TValue>( value );
                }
                catch
                {
                    return default;
                }
            } ).ToArray();
        }

        public ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.setSelectedOptions", elementId, values );
        }

        #endregion

        #endregion

        #region Properties

        protected IJSRuntime Runtime => runtime;

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
