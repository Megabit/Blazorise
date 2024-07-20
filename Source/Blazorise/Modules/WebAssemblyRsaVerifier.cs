#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Licensing.Signing;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the RSA JS module.
    /// </summary>
    internal class WebAssemblyRsaVerifier : BaseJSModule, IVerifier
    {
        #region Members

        private readonly string publicKey;

        private readonly byte[] publicKeyBytes;

        #endregion

        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        /// <param name="options">Blazorise options.</param>
        /// <param name="publicKey">Public RSA key used to generate the license.</param>
        public WebAssemblyRsaVerifier( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options, string publicKey )
            : base( jsRuntime, versionProvider, options )
        {
            this.publicKey = publicKey;
            this.publicKeyBytes = Convert.FromBase64String( publicKey );
        }

        #endregion

        #region Methods

        public async Task<bool> Verify( string content, string signature )
        {
            var bytesSignature = Convert.FromBase64String( signature );
            UnConfuse( bytesSignature );
            signature = Convert.ToBase64String( bytesSignature );

            var result = await InvokeSafeAsync<bool>( "verifyRsa", publicKey, content, signature );

            return result;
        }

        private static void UnConfuse( byte[] bytes )
        {
            for ( int i = 0; i < bytes.Length; i++ )
            {
                bytes[i] ^= Blazorise.Licensing.Constants.ConfusingBytes[i % Blazorise.Licensing.Constants.ConfusingBytes.Length];
            }
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/utilities.js?v={VersionProvider.Version}";

        #endregion        
    }
}
