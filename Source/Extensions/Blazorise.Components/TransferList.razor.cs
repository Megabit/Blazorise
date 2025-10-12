#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Components.ListView;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Represents a component that allows transferring items between two list panes.
/// </summary>
/// <typeparam name="TItem">
/// The type of data item displayed and managed within the transfer lists.
/// </typeparam>
public partial class TransferList<TItem> : ComponentBase
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( Items.IsNullOrEmpty() )
            return;

        var itemsStartStartedNullOrEmpty = ItemsStart.IsNullOrEmpty();
        var itemsEndStartedNullOrEmpty = ItemsEnd.IsNullOrEmpty();

        var bothListsEmpty = itemsStartStartedNullOrEmpty && itemsEndStartedNullOrEmpty;
        var startEmptyButEndNotEmpty = itemsStartStartedNullOrEmpty && !itemsEndStartedNullOrEmpty;
        var startNotEmptyButEndEmpty = !itemsStartStartedNullOrEmpty && itemsEndStartedNullOrEmpty;

        if ( bothListsEmpty )
        {
            // If both lists are empty, then we just start with the items in the start list.
            ItemsStart = Items.ToList();
            ItemsStartChanged.InvokeAsync( ItemsStart );
        }
        else if ( startNotEmptyButEndEmpty )
        {
            // If the start list is not empty, but the end list is empty, then we assign the remainder of existing items to the end list.
            ItemsEnd = Items.Where( x => !ItemsStart.Contains( x ) ).ToList();
            ItemsEndChanged.InvokeAsync( ItemsEnd );
        }
        else if ( startEmptyButEndNotEmpty )
        {
            //If the start list is empty, but the end list is not empty, then we assign the remainder of existing items to the start list.
            ItemsStart = Items.Where( x => !ItemsEnd.Contains( x ) ).ToList();
            ItemsStartChanged.InvokeAsync( ItemsStart );
        }
    }

    /// <summary>
    /// Moves the currently selected item(s) from the start (left) list to the end (right) list,
    /// based on <see cref="SelectionMode"/>.
    /// </summary>
    /// <returns>A task that completes when the move operation finishes.</returns>
    public async Task MoveSelectedEnd()
    {
        ItemsEnd ??= new();

        if ( SelectionMode == ListGroupSelectionMode.Multiple )
        {
            foreach ( var selectedItemStart in SelectedItemsStart )
            {
                ItemsEnd.Add( selectedItemStart );
                ItemsStart.Remove( selectedItemStart );
            }
        }
        else if ( SelectionMode == ListGroupSelectionMode.Single )
        {
            if ( SelectedItemStart is not null )
            {
                ItemsEnd.Add( SelectedItemStart );
                ItemsStart.Remove( SelectedItemStart );
            }
        }

        await NotifyMove( true );
    }

    /// <summary>
    /// Moves the currently selected item(s) from the end (right) list to the start (left) list,
    /// based on <see cref="SelectionMode"/>.
    /// </summary>
    /// <returns>A task that completes when the move operation finishes.</returns>
    public async Task MoveSelectedStart()
    {
        ItemsStart ??= new();

        if ( SelectionMode == ListGroupSelectionMode.Multiple )
        {
            foreach ( var selectedItemEnd in SelectedItemsEnd )
            {
                ItemsStart.Add( selectedItemEnd );
                ItemsEnd.Remove( selectedItemEnd );
            }
        }
        else if ( SelectionMode == ListGroupSelectionMode.Single )
        {
            if ( SelectedItemEnd is not null )
            {
                ItemsStart.Add( SelectedItemEnd );
                ItemsEnd.Remove( SelectedItemEnd );
            }
        }

        await NotifyMove( true );
    }

    /// <summary>
    /// Moves all eligible items from the start (left) list to the end (right) list.
    /// </summary>
    /// <returns>A task that completes when the move operation finishes.</returns>
    public async Task MoveAllEnd()
    {
        ItemsEnd ??= new();

        foreach ( var selectedItemStart in MoveableItemsStart )
        {
            ItemsEnd.Add( selectedItemStart );
        }

        for ( int i = ItemsStart.Count - 1; i >= 0; i-- )
        {
            if ( MoveableItemsStart.Contains( ItemsStart[i] ) )
            {
                ItemsStart.RemoveAt( i );
            }
        }

        await NotifyMove( true );
    }

    /// <summary>
    /// Moves all eligible items from the end (right) list to the start (left) list.
    /// </summary>
    /// <returns>A task that completes when the move operation finishes.</returns>
    public async Task MoveAllStart()
    {
        ItemsStart ??= new();

        foreach ( var selectedItemEnd in MoveableItemsEnd )
        {
            ItemsStart.Add( selectedItemEnd );
        }

        for ( int i = ItemsEnd.Count - 1; i >= 0; i-- )
        {
            if ( MoveableItemsEnd.Contains( ItemsEnd[i] ) )
            {
                ItemsEnd.RemoveAt( i );
            }
        }

        await NotifyMove( true );
    }

    /// <summary>
    /// Notifies item list changes and optionally clears all selections.
    /// </summary>
    /// <param name="clearAllSelections">
    /// If <c>true</c>, clears single- and multi-selection for both lists after notifications.
    /// </param>
    /// <returns>A task that completes when notifications (and optional clearing) finish.</returns>
    protected virtual async Task NotifyMove( bool clearAllSelections )
    {
        await Task.WhenAll(
            ItemsStartChanged.InvokeAsync( ItemsStart ),
            ItemsEndChanged.InvokeAsync( ItemsEnd ) );

        if ( clearAllSelections )
        {
            await Task.WhenAll(
                ClearSelectedItemStart(),
                ClearSelectedItemEnd(),
                ClearSelectedItemsStart(),
                ClearSelectedItemsEnd() );
        }
    }

    /// <summary>
    /// Clears the currently selected item in the start (left) list, if any,
    /// and raises <see cref="SelectedItemStartChanged"/>.
    /// </summary>
    /// <returns>A task that completes when the operation finishes.</returns>
    protected virtual async Task ClearSelectedItemStart()
    {
        if ( !SelectedItemStart.IsEqual( default ) )
        {
            SelectedItemStart = default;

            await SelectedItemStartChanged.InvokeAsync( SelectedItemStart );
        }
    }

    /// <summary>
    /// Clears the currently selected item in the end (right) list, if any,
    /// and raises <see cref="SelectedItemEndChanged"/>.
    /// </summary>
    /// <returns>A task that completes when the operation finishes.</returns>
    protected virtual async Task ClearSelectedItemEnd()
    {
        if ( !SelectedItemEnd.IsEqual( default ) )
        {
            SelectedItemEnd = default;

            await SelectedItemEndChanged.InvokeAsync( SelectedItemEnd );
        }
    }

    /// <summary>
    /// Clears the multi-selection in the start (left) list, if any,
    /// and raises <see cref="SelectedItemsStartChanged"/>.
    /// </summary>
    /// <returns>A task that completes when the operation finishes.</returns>
    protected virtual async Task ClearSelectedItemsStart()
    {
        if ( !SelectedItemsStart.IsNullOrEmpty() )
        {
            SelectedItemsStart.Clear();

            await SelectedItemsStartChanged.InvokeAsync( SelectedItemsStart );
        }
    }

    /// <summary>
    /// Clears the multi-selection in the end (right) list, if any,
    /// and raises <see cref="SelectedItemsEndChanged"/>.
    /// </summary>
    /// <returns>A task that completes when the operation finishes.</returns>
    protected virtual async Task ClearSelectedItemsEnd()
    {
        if ( !SelectedItemsEnd.IsNullOrEmpty() )
        {
            SelectedItemsEnd.Clear();

            await SelectedItemsEndChanged.InvokeAsync( SelectedItemsEnd );
        }
    }

    /// <summary>
    /// Returns whether moving the specified item to the start (left) list is disabled.
    /// </summary>
    /// <param name="item">The item to evaluate.</param>
    /// <returns>
    /// <c>true</c> if movement is disabled by <see cref="CanMoveToStart"/>; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool IsMoveToStartDisabled( TItem item )
    {
        if ( CanMoveToStart is not null )
        {
            return !CanMoveToStart.Invoke( item );
        }

        return false;
    }

    /// <summary>
    /// Returns whether moving the specified item to the end (right) list is disabled.
    /// </summary>
    /// <param name="item">The item to evaluate.</param>
    /// <returns>
    /// <c>true</c> if movement is disabled by <see cref="CanMoveToEnd"/>; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool IsMoveToEndDisabled( TItem item )
    {
        if ( CanMoveToEnd is not null )
        {
            return !CanMoveToEnd.Invoke( item );
        }

        return false;
    }

    /// <summary>
    /// Invoked when the selected item's start value changes.
    /// </summary>
    /// <param name="item">The new item that represents the start of the selected range.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected virtual Task OnSelectedItemStartChanged( TItem item )
    {
        SelectedItemStart = item;

        return SelectedItemStartChanged.InvokeAsync( item );
    }

    /// <summary>
    /// Invoked when the selected item's end value changes.
    /// </summary>
    /// <param name="item">The new selected item of type <typeparamref name="TItem"/>.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected virtual Task OnSelectedItemEndChanged( TItem item )
    {
        SelectedItemEnd = item;

        return SelectedItemEndChanged.InvokeAsync( item );
    }

    /// <summary>
    /// Invoked when the collection of selected items at the start of an operation changes.
    /// </summary>
    /// <param name="items">The new collection of selected items.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected virtual Task OnSelectedItemsStartChanged( List<TItem> items )
    {
        SelectedItemsStart = items;

        return SelectedItemsStartChanged.InvokeAsync( items );
    }

    /// <summary>
    /// Invoked when the selection of items has changed, allowing derived classes to handle the event.
    /// </summary>
    /// <param name="items">The list of items representing the updated selection.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected virtual Task OnSelectedItemsEndChanged( List<TItem> items )
    {
        SelectedItemsEnd = items;

        return SelectedItemsEndChanged.InvokeAsync( items );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the items from the start (left) list that can be moved to the end (right) list.
    /// </summary>
    public IEnumerable<TItem> MoveableItemsStart
        => ItemsStart?.Where( x => !IsMoveToEndDisabled( x ) );

    /// <summary>
    /// Gets the items from the end (right) list that can be moved to the start (left) list.
    /// </summary>
    public IEnumerable<TItem> MoveableItemsEnd
        => ItemsEnd?.Where( x => !IsMoveToStartDisabled( x ) );

    /// <summary>
    /// Gets a value indicating whether the "Move All to End" action is disabled.
    /// </summary>
    public bool IsMoveAllEndDisabled
        => MoveableItemsStart.IsNullOrEmpty();

    /// <summary>
    /// Gets a value indicating whether the "Move All to Start" action is disabled.
    /// </summary>
    public bool IsMoveAllStartDisabled
        => MoveableItemsEnd.IsNullOrEmpty();

    /// <summary>
    /// Gets a value indicating whether the "Move to End" action is disabled based on the current selection mode.
    /// </summary>
    public bool IsMoveEndDisabled => SelectionMode == ListGroupSelectionMode.Single
        ? SelectedItemStart is null
        : SelectionMode == ListGroupSelectionMode.Multiple
            ? SelectedItemsStart.IsNullOrEmpty()
            : false;

    /// <summary>
    /// Gets a value indicating whether the "Move to Start" action is disabled based on the current selection mode.
    /// </summary>
    public bool IsMoveStartDisabled => SelectionMode == ListGroupSelectionMode.Single
        ? SelectedItemEnd is null
        : SelectionMode == ListGroupSelectionMode.Multiple
            ? SelectedItemsEnd.IsNullOrEmpty()
            : true;

    /// <summary>
    /// Defines how the list groups behave (e.g., selectable, static).
    /// </summary>
    [Parameter] public ListGroupMode Mode { get; set; } = ListGroupMode.Selectable;

    /// <summary>
    /// Defines how many items can be selected at once in each list.
    /// </summary>
    [Parameter] public ListGroupSelectionMode SelectionMode { get; set; } = ListGroupSelectionMode.Single;

    /// <summary>
    /// Determines whether the "Move All" buttons are visible between lists.
    /// </summary>
    [Parameter] public bool ShowMoveAll { get; set; } = true;

    /// <summary>
    /// Specifies the color of the move and "Move All" buttons.
    /// </summary>
    [Parameter] public Color MoveButtonsColor { get; set; } = Color.Primary;

    /// <summary>
    /// Enables a vertical scrollbar when the list exceeds the maximum height.
    /// </summary>
    [Parameter] public bool Scrollable { get; set; } = true;

    /// <summary>
    /// Sets the maximum height of each list pane. Defaults to 300px.
    /// </summary>
    [Parameter] public string MaxHeight { get; set; } = "300px";

    /// <summary>
    /// Defines the template used to render items in the start (left) list.
    /// </summary>
    [Parameter] public RenderFragment<ItemContext<TItem>> ItemStartTemplate { get; set; }

    /// <summary>
    /// Defines the template used to render items in the end (right) list.
    /// </summary>
    [Parameter] public RenderFragment<ItemContext<TItem>> ItemEndTemplate { get; set; }

    /// <summary>
    /// Provides the complete collection of items managed by the transfer list.
    /// </summary>
    [EditorRequired][Parameter] public List<TItem> Items { get; set; }

    /// <summary>
    /// Function used to extract the unique value field from an item.
    /// </summary>
    [EditorRequired][Parameter] public Func<TItem, string> ValueField { get; set; }

    /// <summary>
    /// Function used to extract the display text field from an item.
    /// </summary>
    [EditorRequired][Parameter] public Func<TItem, string> TextField { get; set; }

    /// <summary>
    /// Determines whether a specific item can be moved to the start (left) list.
    /// </summary>
    [Parameter] public Func<TItem, bool> CanMoveToStart { get; set; }

    /// <summary>
    /// Determines whether a specific item can be moved to the end (right) list.
    /// </summary>
    [Parameter] public Func<TItem, bool> CanMoveToEnd { get; set; }

    /// <summary>
    /// Gets or sets the items displayed in the start (left) list.
    /// </summary>
    [Parameter] public List<TItem> ItemsStart { get; set; }

    /// <summary>
    /// Event callback triggered when the start list items change.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> ItemsStartChanged { get; set; }

    /// <summary>
    /// Gets or sets the items displayed in the end (right) list.
    /// </summary>
    [Parameter] public List<TItem> ItemsEnd { get; set; }

    /// <summary>
    /// Event callback triggered when the end list items change.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> ItemsEndChanged { get; set; }

    /// <summary>
    /// Gets or sets the currently selected item in the start (left) list.
    /// </summary>
    [Parameter] public TItem SelectedItemStart { get; set; }

    /// <summary>
    /// Event callback triggered when the selected item in the start list changes.
    /// </summary>
    [Parameter] public EventCallback<TItem> SelectedItemStartChanged { get; set; }

    /// <summary>
    /// Gets or sets the currently selected item in the end (right) list.
    /// </summary>
    [Parameter] public TItem SelectedItemEnd { get; set; }

    /// <summary>
    /// Event callback triggered when the selected item in the end list changes.
    /// </summary>
    [Parameter] public EventCallback<TItem> SelectedItemEndChanged { get; set; }

    /// <summary>
    /// Gets or sets the currently selected items in the start (left) list for multi-selection mode.
    /// </summary>
    [Parameter] public List<TItem> SelectedItemsStart { get; set; }

    /// <summary>
    /// Event callback triggered when the selected items in the start list change.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> SelectedItemsStartChanged { get; set; }

    /// <summary>
    /// Gets or sets the currently selected items in the end (right) list for multi-selection mode.
    /// </summary>
    [Parameter] public List<TItem> SelectedItemsEnd { get; set; }

    /// <summary>
    /// Event callback triggered when the selected items in the end list change.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> SelectedItemsEndChanged { get; set; }

    #endregion
}
