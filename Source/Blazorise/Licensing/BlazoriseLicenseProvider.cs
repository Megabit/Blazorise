#region Using directives
using System.ComponentModel;
using System.Reflection;
using Blazorise.Licensing.Signing;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Licensing;

/// <summary>
/// Runs the license validation process.
/// </summary>
public sealed class BlazoriseLicenseProvider
{
    #region Members

    private static readonly Assembly CurrentAssembly = typeof( BlazoriseLicenseProvider ).Assembly;

    private readonly BlazoriseOptions options;

    private readonly IJSRuntime jsRuntime;

    private readonly IVersionProvider versionProvider;

    private readonly BackgroundWorker backgroundWorker;

    private static readonly string PublicKey = "MIGJAoGBAKp0j2x3tjZdwyrL9RD291u2ZQtpq86ggwx1aofZ8Bm3z0RjEaqInKZZQP1eGwlV8u6XOjfYMp2gxUWWziN5t+QikN77GvZcf28EFSvhgIjpXoHzhrDzIAZmJOLOkLft4/XEe3CI0tMVtqmjf5jQbGqY5M/ApqjF1LmDnTWuyFI5AgMBAAE=";

    private static bool initialized = false;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="jsRuntime"></param>
    /// <param name="versionProvider"></param>
    public BlazoriseLicenseProvider( BlazoriseOptions options, IJSRuntime jsRuntime, IVersionProvider versionProvider )
    {
        this.options = options;
        this.jsRuntime = jsRuntime;
        this.versionProvider = versionProvider;

        backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += BackgroundWorker_DoWork;

        backgroundWorker.RunWorkerAsync();
    }

    #endregion

    #region Methods

    private async void BackgroundWorker_DoWork( object sender, DoWorkEventArgs e )
    {
        if ( initialized )
            return;

        if ( string.IsNullOrWhiteSpace( options.ProductToken ) )
        {
            Result = BlazoriseLicenseResult.Community;
            return;
        }

        try
        {
            if ( IsWebAssembly )
            {
                Result = await LicenseVerifier.Create()
                    .WithWebAssemblyRsaPublicKey( jsRuntime, versionProvider, PublicKey )
                    .LoadAndVerify( options.ProductToken, true, new Assembly[] { CurrentAssembly } )
                    ? BlazoriseLicenseResult.Licensed
                    : BlazoriseLicenseResult.Trial;
            }
            else
            {
                Result = await LicenseVerifier.Create()
                    .WithRsaPublicKey( PublicKey )
                    .LoadAndVerify( options.ProductToken, true, new Assembly[] { CurrentAssembly } )
                    ? BlazoriseLicenseResult.Licensed
                    : BlazoriseLicenseResult.Trial;
            }
        }
        catch
        {
            Result = BlazoriseLicenseResult.Trial;
        }
        finally
        {
            initialized = true;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the result of the license validation.
    /// </summary>
    public BlazoriseLicenseResult Result { get; private set; } = BlazoriseLicenseResult.Initializing;

    /// <summary>
    /// Indicates if the current app is running in WebAssembly mode.
    /// </summary>
    private bool IsWebAssembly => jsRuntime is IJSInProcessRuntime;

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