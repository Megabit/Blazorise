namespace Blazorise;

/// <summary>
/// Represents the options available for configuring Blazorise accessibility integrations.
/// </summary>
public class BlazoriseAccessibilityOptions
{
    /// <summary>
    /// If true, <see cref="FieldLabel"/> can automatically render the <c>for</c> attribute for inputs registered in the same <see cref="Field"/>.
    /// </summary>
    public bool UseLabelForAttribute { get; set; } = true;

    /// <summary>
    /// If true, non-labelable controls can automatically render the <c>aria-labelledby</c> attribute from the label component inside the same <see cref="Field"/> or <see cref="Fields"/>.
    /// </summary>
    public bool UseAriaLabelledByAttribute { get; set; } = true;

    /// <summary>
    /// If true, input components can automatically render the <c>aria-invalid</c> attribute from the current validation status.
    /// </summary>
    public bool UseAutoAriaInvalidAttribute { get; set; } = true;

    /// <summary>
    /// If true, input components can automatically render the <c>aria-describedby</c> attribute from the current <see cref="FieldHelp"/> and validation message elements.
    /// </summary>
    public bool UseAutoAriaDescribedByAttribute { get; set; } = true;

    /// <summary>
    /// If true, input components can automatically render the <c>aria-required</c> attribute for required fields.
    /// </summary>
    public bool UseAutoAriaRequiredAttribute { get; set; } = true;

    /// <summary>
    /// If true, input components wrapped in <see cref="Validation"/> can trigger validation when they lose focus, even when the value has not changed.
    /// </summary>
    public bool UseValidationOnBlur { get; set; } = false;
}