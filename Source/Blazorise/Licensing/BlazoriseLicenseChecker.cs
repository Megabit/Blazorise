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
    ///
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
        if ( blazoriseLicenseProvider.Result == BlazoriseLicenseResult.Initializing )
            return false;

        if ( !rendered )
        {
            rendered = true;

            return blazoriseLicenseProvider.Result != BlazoriseLicenseResult.Licensed;
        }

        return false;
    }

    /// <summary>
    /// Returns the maximum number of rows that can be displayed in the data grid.
    /// Null if no limit is set.
    /// </summary>
    /// <returns></returns>
    public int? GetDataGridRowsLimit()
    {
        return blazoriseLicenseProvider.GetDataGridRowsLimit();
    }

    #endregion
}