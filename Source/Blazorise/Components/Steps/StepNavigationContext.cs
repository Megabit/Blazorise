namespace Blazorise;

/// <summary>
/// Defines the context for the step navigation.
/// </summary>
public class StepNavigationContext
{
    /// <summary>
    /// Gets the name of the current step.
    /// </summary>
    public string CurrentStepName { get; init; }

    /// <summary>
    /// Gets the one-based index of the current.
    /// </summary>
    public int CurrentStepIndex { get; init; }

    /// <summary>
    /// Gets the name of the requested step.
    /// </summary>
    public string NextStepName { get; init; }

    /// <summary>
    /// Gets the one-based index of the requested step.
    /// </summary>
    public int NextStepIndex { get; init; }
}