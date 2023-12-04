namespace Blazorise.Licensing;

/// <summary>
/// Defines the type of the license. This mirrors the type defined in the encrypted license.
/// </summary>
public enum BlazoriseLicenseType
{
    /// <summary>
    /// A regular commercial license.
    /// </summary>
    Regular,

    /// <summary>
    /// A commercial trial license.
    /// </summary>
    Trial,

    /// <summary>
    /// A community license.
    /// </summary>
    Community,
}