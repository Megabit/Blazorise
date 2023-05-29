namespace Blazorise.Components;

/// <summary>
/// The Autocomplete's filter mode.
/// </summary>
public enum AutocompleteFilter
{
    /// <summary>
    /// The filtered items will appear only if the search value is found at the beginning of the item value.
    /// </summary>
    StartsWith,

    /// <summary>
    /// The filtered items will appear if the search value is found anywhere in the item value.
    /// </summary>
    Contains,
}
