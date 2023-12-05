﻿#region Using directives
using System;
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

    private int? limitsAutocompleteMaxRows;

    private int? limitsChartsMaxRows;

    private int? limitsListViewMaxRows;

    private int? limitsTreeViewMaxRows;

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

        if ( !initialized )
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerAsync();
        }
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
                var wasmLicenseVerifier = LicenseVerifier.Create().WithWebAssemblyRsaPublicKey( jsRuntime, versionProvider, PublicKey );
                var license = await wasmLicenseVerifier.Load( options.ProductToken, true );

                if ( wasmLicenseVerifier.Verify( license, new Assembly[] { CurrentAssembly } ) )
                {
                    License = license;
                    Result = ResolveBlazoriseLicenseResult( license );
                }
                else
                {
                    Result = BlazoriseLicenseResult.Unlicensed;
                }
            }
            else
            {
                var licenseVerifier = LicenseVerifier.Create().WithRsaPublicKey( PublicKey );
                var license = await licenseVerifier.Load( options.ProductToken, true );

                if ( licenseVerifier.Verify( license, new Assembly[] { CurrentAssembly } ) )
                {
                    License = license;
                    Result = ResolveBlazoriseLicenseResult( license );
                }
                else
                {
                    Result = BlazoriseLicenseResult.Unlicensed;
                }
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

    private static BlazoriseLicenseResult ResolveBlazoriseLicenseResult( License license )
    {
        if ( license is null )
            return BlazoriseLicenseResult.Unlicensed;

        if ( license.Properties.TryGetValue( Constants.Properties.LICENSE_TYPE, out var licenseType ) && Enum.TryParse<BlazoriseLicenseType>( licenseType, true, out var licenseTypeAsEnum ) )
        {
            switch ( licenseTypeAsEnum )
            {
                case BlazoriseLicenseType.Community:
                    return BlazoriseLicenseResult.Community;
                case BlazoriseLicenseType.Regular:
                    return BlazoriseLicenseResult.Licensed;
                case BlazoriseLicenseType.Trial:
                    return BlazoriseLicenseResult.Trial;
                default:
                    break;
            }
        }

        return BlazoriseLicenseResult.Licensed;
    }

    internal int? GetDataGridRowsLimit()
    {
        if ( limitsDataGridMaxRows.HasValue )
            return limitsDataGridMaxRows;

        if ( Result == BlazoriseLicenseResult.Initializing )
            return null;


        if ( License is not null )
        {
            if ( License.Properties.TryGetValue( Constants.Properties.DATAGRID_MAX_ROWS, out var rowsLimitString ) && int.TryParse( rowsLimitString, out var rowsLimit ) )
            {
                limitsDataGridMaxRows = rowsLimit;
            }
        }
        else if ( Result == BlazoriseLicenseResult.Community )
        {
            limitsDataGridMaxRows = 10000;
        }
        else if ( Result == BlazoriseLicenseResult.Unlicensed )
        {
            limitsDataGridMaxRows = 1000;
        }

        return limitsDataGridMaxRows;
    }

    internal int? GetAutocompleteRowsLimit()
    {
        if ( limitsAutocompleteMaxRows.HasValue )
            return limitsAutocompleteMaxRows;

        if ( Result == BlazoriseLicenseResult.Initializing )
            return null;


        if ( License is not null )
        {
            if ( License.Properties.TryGetValue( Constants.Properties.AUTOCOMPLETE_MAX_ROWS, out var rowsLimitString ) && int.TryParse( rowsLimitString, out var rowsLimit ) )
            {
                limitsAutocompleteMaxRows = rowsLimit;
            }
        }
        else if ( Result == BlazoriseLicenseResult.Community )
        {
            limitsAutocompleteMaxRows = 10000;
        }
        else if ( Result == BlazoriseLicenseResult.Unlicensed )
        {
            limitsAutocompleteMaxRows = 1000;
        }

        return limitsAutocompleteMaxRows;
    }

    internal int? GetChartsRowsLimit()
    {
        if ( limitsChartsMaxRows.HasValue )
            return limitsChartsMaxRows;

        if ( Result == BlazoriseLicenseResult.Initializing )
            return null;


        if ( License is not null )
        {
            if ( License.Properties.TryGetValue( Constants.Properties.CHARTS_MAX_ROWS, out var rowsLimitString ) && int.TryParse( rowsLimitString, out var rowsLimit ) )
            {
                limitsChartsMaxRows = rowsLimit;
            }
        }
        else if ( Result == BlazoriseLicenseResult.Community )
        {
            limitsChartsMaxRows = 100;
        }
        else if ( Result == BlazoriseLicenseResult.Unlicensed )
        {
            limitsChartsMaxRows = 10;
        }

        return limitsChartsMaxRows;
    }

    internal int? GetListViewRowsLimit()
    {
        if ( limitsListViewMaxRows.HasValue )
            return limitsListViewMaxRows;

        if ( Result == BlazoriseLicenseResult.Initializing )
            return null;


        if ( License is not null )
        {
            if ( License.Properties.TryGetValue( Constants.Properties.LISTVIEW_MAX_ROWS, out var rowsLimitString ) && int.TryParse( rowsLimitString, out var rowsLimit ) )
            {
                limitsListViewMaxRows = rowsLimit;
            }
        }
        else if ( Result == BlazoriseLicenseResult.Community )
        {
            limitsListViewMaxRows = 100000;
        }
        else if ( Result == BlazoriseLicenseResult.Unlicensed )
        {
            limitsListViewMaxRows = 1000;
        }

        return limitsListViewMaxRows;
    }

    internal int? GetTreeViewRowsLimit()
    {
        if ( limitsTreeViewMaxRows.HasValue )
            return limitsTreeViewMaxRows;

        if ( Result == BlazoriseLicenseResult.Initializing )
            return null;


        if ( License is not null )
        {
            if ( License.Properties.TryGetValue( Constants.Properties.TREEVIEW_MAX_ROWS, out var rowsLimitString ) && int.TryParse( rowsLimitString, out var rowsLimit ) )
            {
                limitsTreeViewMaxRows = rowsLimit;
            }
        }
        else if ( Result == BlazoriseLicenseResult.Community )
        {
            limitsTreeViewMaxRows = 1000;
        }
        else if ( Result == BlazoriseLicenseResult.Unlicensed )
        {
            limitsTreeViewMaxRows = 100;
        }

        return limitsTreeViewMaxRows;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the license.
    /// </summary>
    internal static License License { get; private set; }

    /// <summary>
    /// Gets the result of the license validation.
    /// </summary>
    internal static BlazoriseLicenseResult Result { get; private set; } = BlazoriseLicenseResult.Initializing;

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