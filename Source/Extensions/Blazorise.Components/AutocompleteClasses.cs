using System;
using Blazorise.Components.Autocomplete;

namespace Blazorise.Components;

/// <summary>
/// Component classes for <see cref="Autocomplete{TItem, TValue}"/>.
/// </summary>
/// <typeparam name="TItem">Type of an item filtered by the autocomplete component.</typeparam>
/// <typeparam name="TValue">Type of a SelectedValue field by the autocomplete component.</typeparam>
public sealed record class AutocompleteClasses<TItem, TValue> : ComponentClasses
{
    /// <summary>
    /// Targets the search input element.
    /// </summary>
    public string Input { get; init; }

    /// <summary>
    /// Targets the dropdown menu element.
    /// </summary>
    public string Menu { get; init; }

    /// <summary>
    /// Targets the dropdown item elements based on the current item.
    /// </summary>
    public Func<ItemContext<TItem, TValue>, string> Item { get; init; }

    /// <summary>
    /// Targets the default tag element for multiple selection.
    /// </summary>
    public string Tag { get; init; }
}

/// <summary>
/// Component styles for <see cref="Autocomplete{TItem, TValue}"/>.
/// </summary>
/// <typeparam name="TItem">Type of an item filtered by the autocomplete component.</typeparam>
/// <typeparam name="TValue">Type of a SelectedValue field by the autocomplete component.</typeparam>
public sealed record class AutocompleteStyles<TItem, TValue> : ComponentStyles
{
    /// <summary>
    /// Targets the search input element.
    /// </summary>
    public string Input { get; init; }

    /// <summary>
    /// Targets the dropdown menu element.
    /// </summary>
    public string Menu { get; init; }

    /// <summary>
    /// Targets the dropdown item elements based on the current item.
    /// </summary>
    public Func<ItemContext<TItem, TValue>, string> Item { get; init; }

    /// <summary>
    /// Targets the default tag element for multiple selection.
    /// </summary>
    public string Tag { get; init; }
}