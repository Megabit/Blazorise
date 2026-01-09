namespace Blazorise;

/// <summary>
/// Defines a fluent utility target selector.
/// </summary>
public interface IFluentUtilityTarget<out TSelf>
{
    /// <summary>
    /// Targets the utility output to the component element.
    /// </summary>
    TSelf OnSelf { get; }

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    TSelf OnWrapper { get; }
}