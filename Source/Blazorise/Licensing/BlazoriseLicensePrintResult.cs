namespace Blazorise.Licensing;

/// <summary>
/// Defines the result of the license validation for printing purposes
/// </summary>
public enum BlazoriseLicensePrintResult
{
    /// <summary>
    /// Has not yet been set.
    /// </summary>
    None,

    /// <summary>
    /// License is issued to the community user.
    /// </summary>
    Community,

    /// <summary>
    /// License is issued to the community user, but has expired.
    /// </summary>
    CommunityExpired,

    /// <summary>
    /// License is issued to the commercial user.
    /// </summary>
    Licensed,

    /// <summary>
    /// License is issued to the commercial user, but has expired.
    /// </summary>
    LicensedExpired,

    /// <summary>
    /// License is in trial mode.
    /// </summary>
    Trial,

    /// <summary>
    /// License was unable to be validated.
    /// </summary>
    InvalidProductToken,
}
