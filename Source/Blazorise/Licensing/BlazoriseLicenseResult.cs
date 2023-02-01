namespace Blazorise.Licensing;

/// <summary>
/// Defines the result of the license validation.
/// </summary>
public enum BlazoriseLicenseResult
{
    /// <summary>
    /// License check is still initializing.
    /// </summary>
    Initializing,

    /// <summary>
    /// License is issued to the community user.
    /// </summary>
    Community,

    /// <summary>
    /// License is issued to the commercial user.
    /// </summary>
    Licensed,

    /// <summary>
    /// License is in trial mode.
    /// </summary>
    Trial,
}
