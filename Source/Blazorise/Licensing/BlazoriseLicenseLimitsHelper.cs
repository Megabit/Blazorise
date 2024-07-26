namespace Blazorise.Licensing;

/// <summary>
/// Ultimately provides sane defaults for Blazorise licensing, in case required scoped services are not provided
/// </summary>
public static class BlazoriseLicenseLimitsHelper
{
    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public static int? GetDataGridRowsLimit( BlazoriseLicenseChecker blazoriseLicenseChecker )
    {
        return blazoriseLicenseChecker is null ? BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_DATAGRID_MAX_ROWS : blazoriseLicenseChecker.GetDataGridRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public static int? GetAutocompleteRowsLimit( BlazoriseLicenseChecker blazoriseLicenseChecker )
    {
        return blazoriseLicenseChecker is null ? BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_AUTOCOMPLETE_MAX_ROWS : blazoriseLicenseChecker.GetAutocompleteRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public static int? GetChartsRowsLimit( BlazoriseLicenseChecker blazoriseLicenseChecker )
    {
        return blazoriseLicenseChecker is null ? BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_CHARTS_MAX_ROWS : blazoriseLicenseChecker.GetChartsRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public static int? GetListViewRowsLimit( BlazoriseLicenseChecker blazoriseLicenseChecker )
    {
        return blazoriseLicenseChecker is null ? BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_LISTVIEW_MAX_ROWS : blazoriseLicenseChecker.GetListViewRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public static int? GetTreeViewRowsLimit( BlazoriseLicenseChecker blazoriseLicenseChecker )
    {
        return blazoriseLicenseChecker is null ? BlazoriseLicenseProvider.DEFAULT_UNLICENSED_LIMIT_TREEVIEW_MAX_ROWS : blazoriseLicenseChecker.GetTreeViewRowsLimit();
    }
}
