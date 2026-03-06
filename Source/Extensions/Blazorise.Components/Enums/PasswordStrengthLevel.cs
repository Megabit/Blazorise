namespace Blazorise.Components;

/// <summary>
/// Describes the calculated password strength level.
/// </summary>
public enum PasswordStrengthLevel
{
    /// <summary>
    /// Password is empty or not yet evaluated.
    /// </summary>
    None,

    /// <summary>
    /// Password is weak.
    /// </summary>
    Weak,

    /// <summary>
    /// Password has fair strength.
    /// </summary>
    Fair,

    /// <summary>
    /// Password has good strength.
    /// </summary>
    Good,

    /// <summary>
    /// Password has strong strength.
    /// </summary>
    Strong,
}