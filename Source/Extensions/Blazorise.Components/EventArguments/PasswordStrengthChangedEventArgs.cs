namespace Blazorise.Components;

/// <summary>
/// Provides data for the <see cref="PasswordStrength.StrengthChanged"/> event.
/// </summary>
public class PasswordStrengthChangedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PasswordStrengthChangedEventArgs"/> class.
    /// </summary>
    /// <param name="password">Current password value.</param>
    /// <param name="strengthLevel">Calculated strength level.</param>
    /// <param name="score">Calculated strength score in percentage.</param>
    /// <param name="passedRules">Number of satisfied rules.</param>
    /// <param name="totalRules">Total number of active rules.</param>
    /// <param name="isValid">True if all active rules are satisfied.</param>
    public PasswordStrengthChangedEventArgs( string password, PasswordStrengthLevel strengthLevel, int score, int passedRules, int totalRules, bool isValid )
    {
        Password = password;
        StrengthLevel = strengthLevel;
        Score = score;
        PassedRules = passedRules;
        TotalRules = totalRules;
        IsValid = isValid;
    }

    /// <summary>
    /// Gets the current password value.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Gets the calculated strength level.
    /// </summary>
    public PasswordStrengthLevel StrengthLevel { get; }

    /// <summary>
    /// Gets the calculated strength score in percentage.
    /// </summary>
    public int Score { get; }

    /// <summary>
    /// Gets number of satisfied rules.
    /// </summary>
    public int PassedRules { get; }

    /// <summary>
    /// Gets total number of active rules.
    /// </summary>
    public int TotalRules { get; }

    /// <summary>
    /// Gets true if all active rules are satisfied.
    /// </summary>
    public bool IsValid { get; }
}