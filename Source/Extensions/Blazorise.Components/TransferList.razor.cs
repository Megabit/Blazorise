#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Component for transferring items between two lists.
/// </summary>
/// <typeparam name="TItem">The type of items in the lists.</typeparam>
public partial class TransferList<TItem> : ComponentBase
{
    #region Members

    private TItem selectedListBox1Item;
    private TItem selectedListBox2Item;
    private List<TItem> selectedListBox1Items = new List<TItem>();
    private List<TItem> selectedListBox2Items = new List<TItem>();

    private string selectedValue;
    private List<string> selectedValues = new List<string>();

    #endregion

    #region Methods

    /// <summary>
    /// Moves selected items from the first list to the second list.
    /// </summary>
    private void MoveRight()
    {
        SelectedItems ??= new List<TItem>();
        selectedValues ??= new List<string>();

        if ( SelectionMode == ListGroupSelectionMode.Multiple )
        {
            selectedValues.AddRange( selectedListBox1Items.Select( GetValue ) );
            selectedListBox1Items.Clear();
            selectedListBox2Items.Clear();
        }
        else if ( SelectionMode == ListGroupSelectionMode.Single )
        {
            if ( selectedListBox1Item != null )
            {
                selectedValue = GetValue( selectedListBox1Item );
                selectedListBox1Item = selectedListBox2Item;
            }
        }
    }

    /// <summary>
    /// Moves selected items from the second list to the first list.
    /// </summary>
    private void MoveLeft()
    {
        SelectedItems ??= new List<TItem>();
        selectedValues ??= new List<string>();

        if ( SelectionMode == ListGroupSelectionMode.Multiple )
        {
            foreach ( var value in selectedListBox2Items.ToList() )
            {
                selectedValues.Remove( GetValue( value ) );
                selectedListBox2Items.Remove( value );
            }
        }
        else if ( SelectionMode == ListGroupSelectionMode.Single )
        {
            selectedValue = null;
            selectedListBox2Item = selectedListBox1Item;
        }
    }

    /// <summary>
    /// Moves all items from the first list to the second list.
    /// </summary>
    private void MoveAllRight()
    {
        SelectedItems ??= new List<TItem>();
        selectedValues ??= new List<string>();

        if ( SelectionMode == ListGroupSelectionMode.Multiple )
        {
            var itemsToAdd = Items.Where( item => !selectedValues.Contains( GetValue( item ) ) ).ToList();

            selectedValues.AddRange( itemsToAdd.Select( GetValue ) );
            selectedListBox2Items.AddRange( itemsToAdd );
            selectedListBox2Items.Clear();
        }
    }

    /// <summary>
    /// Moves all items from the second list to the first list.
    /// </summary>
    private void MoveAllLeft()
    {
        SelectedItems ??= new List<TItem>();
        selectedValues ??= new List<string>();

        if ( SelectionMode == ListGroupSelectionMode.Multiple )
        {
            var itemsToRemove = Items.Where( item => selectedValues.Contains( GetValue( item ) ) ).ToList();

            selectedValues.RemoveAll( value => itemsToRemove.Any( item => GetValue( item ) == value ) );
            selectedListBox1Items.AddRange( itemsToRemove );
            selectedListBox2Items.RemoveAll( item => itemsToRemove.Contains( item ) );
            selectedListBox1Items.Clear();
        }
    }

    /// <summary>
    /// Get value from list view item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    string GetValue( TItem item ) => ValueField?.Invoke( item );

    #endregion

    #region Properties

    /// <summary>
    /// Represents a collection of items from first list view based on the specified filtering logic.
    /// </summary>
    IEnumerable<TItem> Filtered1Items => SelectionMode == ListGroupSelectionMode.Single
        ? Items.Where( x => GetValue( x ) != selectedValue )
        : Items.Where( x => !( selectedValues ?? Enumerable.Empty<string>() ).Contains( GetValue( x ) ) );

    /// <summary>
    /// Represents a collection of items from second list view based on different filtering logic.
    /// </summary>
    IEnumerable<TItem> Filtered2Items => SelectionMode == ListGroupSelectionMode.Single
        ? Items.Where( x => GetValue( x ) == selectedValue )
        : Items.Where( x => ( selectedValues ?? Enumerable.Empty<string>() ).Contains( GetValue( x ) ) );

    /// <summary>
    /// Gets a value indicating whether the "Move All Right" action is disabled.
    /// </summary>
    bool IsMoveAllRightDisabled => SelectionMode == ListGroupSelectionMode.Multiple
        ? selectedValues.Count == Items.Count
        : false;

    /// <summary>
    /// Gets a value indicating whether the "Move All Left" action is disabled.
    /// </summary>
    bool IsMoveAllLeftDisabled => SelectionMode == ListGroupSelectionMode.Multiple
        ? selectedValues.Count == 0
        : false;

    /// <summary>
    /// Gets a value indicating whether the "Move Right" action is disabled.
    /// </summary>
    bool IsMoveRightDisabled => SelectionMode == ListGroupSelectionMode.Single
        ? selectedListBox1Item is null
        : SelectionMode == ListGroupSelectionMode.Multiple
            ? selectedListBox1Items.Count == 0
            : false;

    /// <summary>
    /// Gets a value indicating whether the "Move Left" action is disabled.
    /// </summary>
    bool IsMoveLeftDisabled => SelectionMode == ListGroupSelectionMode.Single
        ? selectedListBox2Item is null
        : SelectionMode == ListGroupSelectionMode.Multiple
            ? selectedListBox2Items.Count == 0
            : true;

    /// <summary>
    /// Defines the list-group behavior mode.
    /// </summary>
    [Parameter] public ListGroupMode Mode { get; set; } = ListGroupMode.Selectable;

    /// <summary>
    /// Defines the list-group selection mode.
    /// </summary>
    [Parameter] public ListGroupSelectionMode SelectionMode { get; set; } = ListGroupSelectionMode.Single;

    /// <summary>
    /// Makes the list group scrollable by adding a vertical scrollbar.
    /// </summary>
    [Parameter] public bool Scrollable { get; set; } = true;

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the items in the list.
    /// </summary>
    [Parameter] public List<TItem> Items { get; set; }

    /// <summary>
    /// Gets or sets item that is currently selected.
    /// </summary>
    [Parameter] public TItem SelectedItem { get; set; }

    /// <summary>
    /// Gets or sets items that are currently selected.
    /// </summary>
    [Parameter] public List<TItem> SelectedItems { get; set; }

    /// <summary>
    /// Gets or sets the event callback for changes to the items in the list.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> SelectedItemsChanged { get; set; }

    /// <summary>
    /// Gets or sets the function to extract the value field from an item.
    /// </summary>
    [Parameter] public Func<TItem, string> ValueField { get; set; }

    /// <summary>
    /// Gets or sets the function to extract the text field from an item.
    /// </summary>
    [Parameter] public Func<TItem, string> TextField { get; set; }

    #endregion
}
