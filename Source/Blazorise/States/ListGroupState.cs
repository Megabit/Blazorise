using System.Collections.Generic;

namespace Blazorise.States;

/// <summary>
/// Holds the information about the current state of the <see cref="ListGroup"/> component.
/// </summary>
public record ListGroupState
{
    /// <summary>
    /// Remove some borders and rounded corners to render list group items edge-to-edge in a parent container (e.g., cards).
    /// </summary>
    public bool Flush { get; init; }

    /// <summary>
    /// Makes the list group scrollable by adding a vertical scrollbar.
    /// </summary>
    public bool Scrollable { get; init; }

    /// <summary>
    /// Defines the list-group behaviour mode.
    /// </summary>
    public ListGroupMode Mode { get; init; }

    /// <summary>
    /// Defines the list-group selection mode.
    /// </summary>
    public ListGroupSelectionMode SelectionMode { get; init; }

    /// <summary>
    /// Gets or sets currently selected item name.
    /// </summary>
    public string SelectedItem { get; init; }

    /// <summary>
    /// Gets or sets currently selected items.
    /// </summary>
    public List<string> SelectedItems { get; init; }
}