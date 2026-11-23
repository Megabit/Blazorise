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
        if ( IsInitializing )
        {
            return false;
        }

        if ( !rendered )
        {
            rendered = true;

            return blazoriseLicenseProvider.Result != BlazoriseLicenseResult.Licensed;
        }

        return false;
    }

    internal bool ShowBanner()
    {
        return !( blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.Licensed
            || blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.Community
            || blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.CommunityExpired
            || blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.Trial );
    }

    internal bool ShowCompactBanner()
    {
        return blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.Community
            || blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.CommunityExpired;
    }

    internal string GetPrintMessage()
    {
        if ( blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.InvalidProject )
        {
            var projectDescription = string.IsNullOrWhiteSpace( blazoriseLicenseProvider.ProjectName )
                ? "the registered project"
                : $"project '{blazoriseLicenseProvider.ProjectName}'";

            return $"The Indie ProductToken for {projectDescription} does not cover this application or has invalid project metadata. Set BlazoriseOptions.ProductAssembly to a registered application assembly and check the token's project name and application assemblies. Evaluation limits apply. See https://blazorise.com/docs/usage/licensing/register-product-token.";
        }

        if ( blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.Community )
        {
            return "Thank you for using the Blazorise component library with a Community License, which is free for individual use. For additional features and commercial use, consider upgrading your license at https://blazorise.com/commercial. We appreciate your support!";
        }

        if ( blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.CommunityExpired )
        {
            return "Your Community License for the Blazorise component library has expired. To continue using our library and receive updates, please renew your license at https://blazorise.com/acount. Thank you for your continued interest in Blazorise!";
        }

        if ( blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.TrialExpired )
        {
            return "Your trial period for the Blazorise component library has expired. To continue using our library and access premium features, please consider purchasing a commercial license at https://blazorise.com/commercial. We appreciate your interest in Blazorise and look forward to serving you!";
        }

        if ( blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.LicensedExpired )
        {
            return "Your commercial ProductToken for the Blazorise component library has expired. Visit https://blazorise.com/account to check your license and update your ProductToken.";
        }

        if ( blazoriseLicenseProvider.PrintResult == BlazoriseLicensePrintResult.InvalidProductToken )
        {
            return "We've detected an invalid product token for your Blazorise component library. Please make sure your token is correct or visit https://blazorise.com/support for help. Protecting your software investment is important to us!";
        }

        return "You are evaluating the Blazorise component library. Evaluation is free with no time limit and requires no registration or approval. Evaluation component limits and licensing notices apply. Choose Indie for an eligible project, Professional, or Enterprise at https://blazorise.com/pricing.";
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
    public int? GetAutocompleteRowsLimit()
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

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetPivotGridRowsLimit()
    {
        return blazoriseLicenseProvider.GetPivotGridRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetGanttRowsLimit()
    {
        return blazoriseLicenseProvider.GetGanttRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be rendered in a report.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetReportingRowsLimit()
    {
        return blazoriseLicenseProvider.GetReportingRowsLimit();
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetTransferListRowsLimit()
    {
        return blazoriseLicenseProvider.GetTransferListRowsLimit();
    }

    #endregion

    #region Properties

    private bool IsInitializing => blazoriseLicenseProvider.Result == BlazoriseLicenseResult.Initializing;

    #endregion
}