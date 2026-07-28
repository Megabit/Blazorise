namespace Blazorise;

/// <summary>
/// Defines an option rendered by a property grid select editor.
/// </summary>
/// <typeparam name="TValue">The option value type.</typeparam>
/// <param name="Value">The option value.</param>
/// <param name="Text">The option display text.</param>
/// <param name="Disabled">Whether the option is disabled.</param>
public sealed record PropertyGridSelectOption<TValue>( TValue Value, string Text, bool Disabled = false );
