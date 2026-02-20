namespace Blazorise.Components;

/// <summary>
/// Component classes for <see cref="PasswordStrength"/>.
/// </summary>
public sealed record class PasswordStrengthClasses : ComponentClasses
{
    /// <summary>
    /// Targets the password strength container element.
    /// </summary>
    public string Container { get; init; }

    /// <summary>
    /// Targets the addons wrapper around the input and toggle button.
    /// </summary>
    public string Addons { get; init; }

    /// <summary>
    /// Targets the inner text input element.
    /// </summary>
    public string Input { get; init; }

    /// <summary>
    /// Targets the password visibility toggle button element.
    /// </summary>
    public string ToggleButton { get; init; }

    /// <summary>
    /// Targets the strength row element.
    /// </summary>
    public string StrengthContainer { get; init; }

    /// <summary>
    /// Targets the strength caption element.
    /// </summary>
    public string StrengthCaption { get; init; }

    /// <summary>
    /// Targets the current strength value element.
    /// </summary>
    public string StrengthValue { get; init; }

    /// <summary>
    /// Targets the progress wrapper element.
    /// </summary>
    public string ProgressContainer { get; init; }

    /// <summary>
    /// Targets the progress bar element.
    /// </summary>
    public string Progress { get; init; }

    /// <summary>
    /// Targets the rule list wrapper element.
    /// </summary>
    public string RulesContainer { get; init; }

    /// <summary>
    /// Targets each rule row element.
    /// </summary>
    public string RuleItem { get; init; }

    /// <summary>
    /// Targets the rule icon element.
    /// </summary>
    public string RuleIcon { get; init; }

    /// <summary>
    /// Targets the rule text element.
    /// </summary>
    public string RuleText { get; init; }
}

/// <summary>
/// Component styles for <see cref="PasswordStrength"/>.
/// </summary>
public sealed record class PasswordStrengthStyles : ComponentStyles
{
    /// <summary>
    /// Targets the password strength container element.
    /// </summary>
    public string Container { get; init; }

    /// <summary>
    /// Targets the addons wrapper around the input and toggle button.
    /// </summary>
    public string Addons { get; init; }

    /// <summary>
    /// Targets the inner text input element.
    /// </summary>
    public string Input { get; init; }

    /// <summary>
    /// Targets the password visibility toggle button element.
    /// </summary>
    public string ToggleButton { get; init; }

    /// <summary>
    /// Targets the strength row element.
    /// </summary>
    public string StrengthContainer { get; init; }

    /// <summary>
    /// Targets the strength caption element.
    /// </summary>
    public string StrengthCaption { get; init; }

    /// <summary>
    /// Targets the current strength value element.
    /// </summary>
    public string StrengthValue { get; init; }

    /// <summary>
    /// Targets the progress wrapper element.
    /// </summary>
    public string ProgressContainer { get; init; }

    /// <summary>
    /// Targets the progress bar element.
    /// </summary>
    public string Progress { get; init; }

    /// <summary>
    /// Targets the rule list wrapper element.
    /// </summary>
    public string RulesContainer { get; init; }

    /// <summary>
    /// Targets each rule row element.
    /// </summary>
    public string RuleItem { get; init; }

    /// <summary>
    /// Targets the rule icon element.
    /// </summary>
    public string RuleIcon { get; init; }

    /// <summary>
    /// Targets the rule text element.
    /// </summary>
    public string RuleText { get; init; }
}