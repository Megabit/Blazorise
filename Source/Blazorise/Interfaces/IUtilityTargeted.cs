namespace Blazorise;

/// <summary>
/// Defines an optional utility target override for fluent utility builders.
/// </summary>
public interface IUtilityTargeted
{
    /// <summary>
    /// Gets or sets the target where the utility output is applied.
    /// </summary>
    UtilityTarget? UtilityTarget { get; set; }
}