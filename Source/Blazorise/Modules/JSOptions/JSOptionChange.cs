namespace Blazorise.Modules;

/// <summary>
/// Represents a change in an option value with a flag indicating whether the value has changed and the new value.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <param name="Changed">Indicates whether the option value has changed.</param>
/// <param name="Value">The new value of the option.</param>
public record JSOptionChange<TValue>( bool Changed, TValue Value );
