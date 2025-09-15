#region Using directives
using System;
using System.ComponentModel;
using System.Reflection;
using Blazorise.Licensing.Signing;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Licensing;

/// <summary>
/// Runs the license validation process.
/// </summary>
public sealed class BlazoriseLicenseProvider
{
    #region Members

    /// <summary>
    /// The default maximum number of rows allowed in a data grid for unlicensed users.
    /// </summary>
    public const int DEFAULT_UNLICENSED_LIMIT_DATAGRID_MAX_ROWS = 1000;

    /// <summary>
    /// The default maximum number of rows returned by the autocomplete feature for unlicensed users.
    /// </summary>
    public const int DEFAULT_UNLICENSED_LIMIT_AUTOCOMPLETE_MAX_ROWS = 1000;

    /// <summary>
    /// The default maximum number of rows allowed in charts for unlicensed users.
    /// </summary>
    public const int DEFAULT_UNLICENSED_LIMIT_CHARTS_MAX_ROWS = 10;

    /// <summary>
    /// The default maximum number of rows that can be displayed in a list view for unlicensed users.
    /// </summary>
    public const int DEFAULT_UNLICENSED_LIMIT_LISTVIEW_MAX_ROWS = 1000;

    /// <summary>
    /// The default maximum number of rows displayed in a tree view for unlicensed users.
    /// </summary>
    public const int DEFAULT_UNLICENSED_LIMIT_TREEVIEW_MAX_ROWS = 100;

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
    /// A default <see cref="BlazoriseLicenseProvider"/> constructor.
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
                var wasmLicenseVerifier = LicenseVerifier.Create().WithWebAssemblyRsaPublicKey( jsRuntime, versionProvider, options, PublicKey );
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

                PrintResult = ResolveBlazoriseLicensePrintResult( license );
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

                PrintResult = ResolveBlazoriseLicensePrintResult( license );
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

    /// <summary>
    /// Resolves the print result of the license validation by checking the license type and whether it is expired by checking against the actual internal resolved License Result state.
    /// </summary>
    /// <param name="license"></param>
    /// <returns></returns>
    private static BlazoriseLicensePrintResult ResolveBlazoriseLicensePrintResult( License license )
    {
        var licenseResult = ResolveBlazoriseLicenseResult( license );

        if ( licenseResult == BlazoriseLicenseResult.Unlicensed )
            return BlazoriseLicensePrintResult.InvalidProductToken;

        if ( licenseResult == BlazoriseLicenseResult.Community )
        {
            return Result == BlazoriseLicenseResult.Community ? BlazoriseLicensePrintResult.Community : BlazoriseLicensePrintResult.CommunityExpired;
        }

        if ( licenseResult == BlazoriseLicenseResult.Licensed )
        {
            return Result == BlazoriseLicenseResult.Licensed ? BlazoriseLicensePrintResult.Licensed : BlazoriseLicensePrintResult.LicensedExpired;
        }

        if ( licenseResult == BlazoriseLicenseResult.Trial )
            return Result == BlazoriseLicenseResult.Trial ? BlazoriseLicensePrintResult.Trial : BlazoriseLicensePrintResult.TrialExpired;

        return BlazoriseLicensePrintResult.None;
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
            limitsDataGridMaxRows = DEFAULT_UNLICENSED_LIMIT_DATAGRID_MAX_ROWS;
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
            limitsAutocompleteMaxRows = DEFAULT_UNLICENSED_LIMIT_AUTOCOMPLETE_MAX_ROWS;
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
            limitsChartsMaxRows = DEFAULT_UNLICENSED_LIMIT_CHARTS_MAX_ROWS;
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
            limitsListViewMaxRows = DEFAULT_UNLICENSED_LIMIT_LISTVIEW_MAX_ROWS;
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
            limitsTreeViewMaxRows = DEFAULT_UNLICENSED_LIMIT_TREEVIEW_MAX_ROWS;
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
    /// Gets the print result of the license validation.
    /// </summary>
    internal static BlazoriseLicensePrintResult PrintResult { get; private set; } = BlazoriseLicensePrintResult.None;

    /// <summary>
    /// Indicates if the current app is running in WebAssembly mode.
    /// </summary>
    private bool IsWebAssembly => jsRuntime is IJSInProcessRuntime;

    #endregion
}