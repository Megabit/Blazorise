namespace Blazorise;

/// <summary>
/// Defines the validation results.
/// </summary>
public enum ValidationStatus
{
    /// <summary>
    /// No validation.
    /// </summary>
    None,

    /// <summary>
    /// Validation has passed the check.
    /// </summary>
    Success,

    /// <summary>
    /// Validation has warnings.
    /// </summary>
    Warning,

    /// <summary>
    /// Validation has failed.
    /// </summary>
    Error,
}