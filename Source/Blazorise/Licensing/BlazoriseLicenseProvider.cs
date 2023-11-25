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

    private int? limitsDataGridMaxRows;

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
        {
            if ( backgroundWorker is not null )
            {
                backgroundWorker.DoWork -= BackgroundWorker_DoWork;
                backgroundWorker.Dispose();
            }
            return;
        }

        if ( string.IsNullOrWhiteSpace( options.ProductToken ) )
        {
            Result = BlazoriseLicenseResult.Unlicensed;
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

                License = await LicenseVerifier.Create()
                    .WithWebAssemblyRsaPublicKey( jsRuntime, versionProvider, PublicKey )
                    .Load( options.ProductToken, true );
            }
            else
            {
                Result = await LicenseVerifier.Create()
                    .WithRsaPublicKey( PublicKey )
                    .LoadAndVerify( options.ProductToken, true, new Assembly[] { CurrentAssembly } )
                    ? BlazoriseLicenseResult.Licensed
                    : BlazoriseLicenseResult.Trial;

                //TODO: Only Load once.
                //Add a verify method on License that returns the BlazoriseLicenseResult instead of a bool.
                License = await LicenseVerifier.Create()
                    .WithRsaPublicKey( PublicKey )
                    .Load( options.ProductToken, true );
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

    internal int? GetDataGridRowsLimit()
    {
        if ( limitsDataGridMaxRows.HasValue )
            return limitsDataGridMaxRows;

        var initOrLicensedOrTrial =
            Result == BlazoriseLicenseResult.Initializing
            || Result == BlazoriseLicenseResult.Licensed
            || Result == BlazoriseLicenseResult.Trial;

        if ( initOrLicensedOrTrial )
            return null;


        if ( License is not null )
        {
            if ( License.Properties.TryGetValue( "__LIMITS__DATAGRID__MAX_ROWS__", out var rowsLimitString ) && int.TryParse( rowsLimitString, out var rowsLimit ) )
            {
                limitsDataGridMaxRows = rowsLimit;
            }
        }
        else if ( Result == BlazoriseLicenseResult.Community )
        {
            limitsDataGridMaxRows = 100000;
        }
        else if ( Result == BlazoriseLicenseResult.Unlicensed )
        {
            limitsDataGridMaxRows = 1000;
        }

        return limitsDataGridMaxRows;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the license.
    /// </summary>
    internal License License { get; private set; }

    /// <summary>
    /// Gets the result of the license validation.
    /// </summary>
    internal BlazoriseLicenseResult Result { get; private set; } = BlazoriseLicenseResult.Initializing;

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