#region Using directives
using System;
using System.ComponentModel;
using System.Reflection;
using Blazorise.Licensing;
using Blazorise.Licensing.Signing;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    internal class BlazoriseLicenseProvider
    {
        private static readonly Assembly CurrentAssembly = typeof( BlazoriseLicenseProvider ).Assembly;

        private readonly BlazoriseOptions options;

        private readonly IJSRuntime jsRuntime;

        private readonly IVersionProvider versionProvider;

        private readonly BackgroundWorker backgroundWorker;

        private static readonly string PublicKey = "BgIAAACkAABSU0ExAAQAAAEAAQClrL7lCAg7/SyES2q0zM+EN4lFwSQXznI4ItGRtMsMcSJgOGdTkF+hzR/iEVflVSAYe8z8dbNiucNJjWqFsO4o3eMIqn2CHgFZwhZZM2JIpVmqNkoLaUTo+WmlV0YIadWWSzi8UZAUS+hqHjTCPQFkB4FnWlB8d8apMJbVvxKwxg==";

        public BlazoriseLicenseProvider( BlazoriseOptions options, IJSRuntime jsRuntime, IVersionProvider versionProvider )
        {
            this.options = options;
            this.jsRuntime = jsRuntime;
            this.versionProvider = versionProvider;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;

            backgroundWorker.RunWorkerAsync();
        }

        private async void BackgroundWorker_DoWork( object sender, DoWorkEventArgs e )
        {
            if ( string.IsNullOrWhiteSpace( options.LicenseKey ) )
            {
                Result = BlazoriseLicenseResult.Community;
                return;
            }

            try
            {
                if ( IsWebAssembly )
                {
                    Result = await LicenceVerifier.Create()
                        .WithWebAssemblyRsaPublicKey( jsRuntime, versionProvider, PublicKey )
                        .LoadAndVerify( options.LicenseKey, true, new Assembly[] { CurrentAssembly } )
                        ? BlazoriseLicenseResult.Licensed
                        : BlazoriseLicenseResult.Trial;
                }
                else
                {
                    Result = await LicenceVerifier.Create()
                        .WithRsaPublicKey( PublicKey )
                        .LoadAndVerify( options.LicenseKey, true, new Assembly[] { CurrentAssembly } )
                        ? BlazoriseLicenseResult.Licensed
                        : BlazoriseLicenseResult.Trial;
                }
            }
            catch ( Exception exc )
            {
                Result = BlazoriseLicenseResult.Trial;
            }
        }

        public BlazoriseLicenseResult Result { get; private set; } = BlazoriseLicenseResult.Initializing;

        #region Properties

        /// <summary>
        /// Indicates if the current app is running in webassembly mode.
        /// </summary>
        protected bool IsWebAssembly => jsRuntime is IJSInProcessRuntime;

        #endregion
    }

    internal static class WebAssemblyRsaExtensions
    {
        public static IVerifier_LoadAndVerify WithWebAssemblyRsaPublicKey( this IVerifier_WithVerifier signer, IJSRuntime jsRuntime, IVersionProvider versionProvider, string base64EncodedCsbBlobKey )
        {
            var rsaVerifier = new WebAssemblyRsaVerifier( jsRuntime, versionProvider, base64EncodedCsbBlobKey );
            signer.WithVerifier( rsaVerifier );
            return signer as IVerifier_LoadAndVerify;
        }
    }
}