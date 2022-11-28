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

/// <summary>
/// The Autocomplete's selection mode.
/// </summary>
public enum AutocompleteSelectionMode
{
    /// <summary>
    /// Default mode. Selection is single.
    /// </summary>
    Default,

    /// <summary>
    /// Multiple mode. Selection is multiple.
    /// </summary>
    Multiple,

    /// <summary>
    /// Checkbox mode. Selection is multiple with checkbox selection support.
    /// </summary>
    Checkbox
}