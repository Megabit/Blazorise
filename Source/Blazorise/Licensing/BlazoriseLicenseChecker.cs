namespace Blazorise.Licensing;

/// <summary>
/// Checks the validation for the current user session.
/// </summary>
public sealed class BlazoriseLicenseChecker
{
    #region Members

    private readonly BlazoriseLicenseProvider blazoriseLicenseProvider;

    private bool rendered;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="BlazoriseLicenseChecker"/> constructor.
    /// </summary>
    /// <param name="blazoriseLicenseProvider"></param>
    public BlazoriseLicenseChecker( BlazoriseLicenseProvider blazoriseLicenseProvider )
    {
        this.blazoriseLicenseProvider = blazoriseLicenseProvider;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Indicates if the license warning should be displayed.
    /// </summary>
    /// <returns>True if the license warning should be displayed.</returns>
    internal bool ShouldPrint()
    {
        if ( BlazoriseLicenseProvider.Result == BlazoriseLicenseResult.Initializing )
            return false;

        if ( !rendered )
        {
            rendered = true;

            return BlazoriseLicenseProvider.Result != BlazoriseLicenseResult.Licensed;
        }

        return false;
    }

    internal string GetPrintMessage()
    {
        if ( BlazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.Community )
        {
            return "Thank you for using the Blazorise component library with a Community License, which is free for individual use. For additional features and commercial use, consider upgrading your license at https://blazorise.com/commercial. We appreciate your support!";
        }

        if ( BlazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.CommunityExpired )
        {
            return "Your Community License for the Blazorise component library has expired. To continue using our library and receive updates, please renew your license at https://blazorise.com/acount. Thank you for your continued interest in Blazorise!";
        }

        if ( BlazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.LicensedExpired )
        {
            return "Your Commercial License for the Blazorise component library has expired. Please renew your license at https://blazorise.com/account to maintain access to premium features and support. We appreciate your business and look forward to continuing our partnership!";
        }

        if ( BlazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.InvalidProductToken )
        {
            return "We've detected an invalid product token for your Blazorise component library. Please make sure your token is correct or visit https://blazorise.com/support for help. Protecting your software investment is important to us!";
        }

        return "Thank you for using the free version of the Blazorise component library! We're happy to offer it to you for personal use. If you'd like to remove this message, consider purchasing a commercial license from https://blazorise.com/commercial. We appreciate your support!";
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetDataGridRowsLimit()
    {
        return blazoriseLicenseProvider.GetDataGridRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetAutoCompleteRowsLimit()
    {
        return blazoriseLicenseProvider.GetAutocompleteRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetChartsRowsLimit()
    {
        return blazoriseLicenseProvider.GetChartsRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetListViewRowsLimit()
    {
        return blazoriseLicenseProvider.GetListViewRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetTreeViewRowsLimit()
    {
        return blazoriseLicenseProvider.GetTreeViewRowsLimit();
    }

    #endregion
}