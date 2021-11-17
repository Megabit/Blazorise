#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the <see cref="Select{TValue}"/> JS module.
    /// </summary>
    public class JSSelectModule : BaseJSModule, IJSSelectModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSSelectModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "setSelectedOptions", elementId, values );
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId )
        {
            var moduleInstance = await Module;

            // All of this is because Blazor is not serializing types as it should! In this case nullable types
            // are not working (enum?, int?, etc.) so we need to do it manually.

            // Get the selected values from JS as strings.
            var stringValues = await moduleInstance.InvokeAsync<string[]>( "getSelectedOptions", elementId );

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

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/select.js?v={VersionProvider.Version}";

        #endregion
    }
}
