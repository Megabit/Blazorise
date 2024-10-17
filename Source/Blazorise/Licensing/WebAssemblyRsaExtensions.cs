#region Using directives
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Licensing;

internal static class WebAssemblyRsaExtensions
{
    public static IVerifier_LoadAndVerify WithWebAssemblyRsaPublicKey( this IVerifier_WithVerifier signer, IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options, string base64EncodedCsbBlobKey )
    {
        var rsaVerifier = new WebAssemblyRsaVerifier( jsRuntime, versionProvider, options, base64EncodedCsbBlobKey );
        signer.WithVerifier( rsaVerifier );
        return signer as IVerifier_LoadAndVerify;
    }
}