namespace Blazorise;

/// <summary>
/// Base class for all theme options.
/// </summary>
public record ThemeBasicOptions
{
    /// <summary>
    /// Defines the size of the element border radius.
    /// </summary>
    public string BorderRadius { get; set; } = ".25rem";

    /// <summary>
    /// Checks if the options has any option attribute defined.
    /// </summary>
    /// <returns>True if at least one attribute is defined.</returns>
    public virtual bool HasOptions()
    {
        return !string.IsNullOrEmpty( BorderRadius );
    }
}