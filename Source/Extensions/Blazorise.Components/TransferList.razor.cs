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
/// Component for transferring items between two lists.
/// </summary>
/// <typeparam name="TItem">The type of items in the lists.</typeparam>
public partial class TransferList<TItem> : ComponentBase
{
    #region Members

    #endregion

    #region Methods

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
            ItemsStart = Items;
            ItemsStartChanged.InvokeAsync( Items );
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
            ItemsStartChanged.InvokeAsync( ItemsEnd );
        }
    }

    /// <summary>
    /// Moves selected items from the start list to the end list.
    /// </summary>
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
    /// Moves selected items from the end list to the start list.
    /// </summary>
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
    /// Moves all items from the start list to the end list.
    /// </summary>
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
    /// Moves all items from the end list to the start list.
    /// </summary>
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

    private async Task NotifyMove( bool clearAllSelections )
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

    private async Task ClearSelectedItemStart()
    {
        if ( !SelectedItemStart.IsEqual( default ) )
        {
            SelectedItemStart = default;
            await SelectedItemStartChanged.InvokeAsync( SelectedItemStart );
        }
    }

    private async Task ClearSelectedItemEnd()
    {
        if ( !SelectedItemEnd.IsEqual( default ) )
        {
            SelectedItemEnd = default;
            await SelectedItemEndChanged.InvokeAsync( SelectedItemEnd );
        }
    }

    private async Task ClearSelectedItemsStart()
    {
        if ( !SelectedItemsStart.IsNullOrEmpty() )
        {
            SelectedItemsStart.Clear();
            await SelectedItemsStartChanged.InvokeAsync( SelectedItemsStart );
        }
    }

    private async Task ClearSelectedItemsEnd()
    {
        if ( !SelectedItemsEnd.IsNullOrEmpty() )
        {
            SelectedItemsEnd.Clear();
            await SelectedItemsEndChanged.InvokeAsync( SelectedItemsEnd );
        }
    }

    private bool IsMoveToStartDisabled( TItem item )
    {
        if ( CanMoveToStart is not null )
            return !CanMoveToStart.Invoke( item );

        return false;
    }

    private bool IsMoveToEndDisabled( TItem item )
    {
        if ( CanMoveToEnd is not null )
            return !CanMoveToEnd.Invoke( item );

        return false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns the items that can be moved to the end list.
    /// </summary>
    public IEnumerable<TItem> MoveableItemsStart
        => ItemsStart?.Where( x => !IsMoveToEndDisabled( x ) );

    /// <summary>
    /// Returns the items that can be moved to the start list.
    /// </summary>
    public IEnumerable<TItem> MoveableItemsEnd
        => ItemsEnd?.Where( x => !IsMoveToStartDisabled( x ) );

    /// <summary>
    /// Gets a value indicating whether the "Move All End" action is disabled.
    /// </summary>
    public bool IsMoveAllEndDisabled
        => MoveableItemsStart.IsNullOrEmpty();

    /// <summary>
    /// Gets a value indicating whether the "Move All Start" action is disabled.
    /// </summary>
    public bool IsMoveAllStartDisabled
        => MoveableItemsEnd.IsNullOrEmpty();

    /// <summary>
    /// Gets a value indicating whether the "Move End" action is disabled.
    /// </summary>
    public bool IsMoveEndDisabled => SelectionMode == ListGroupSelectionMode.Single
        ? SelectedItemStart is null
        : SelectionMode == ListGroupSelectionMode.Multiple
            ? SelectedItemsStart.IsNullOrEmpty()
            : false;

    /// <summary>
    /// Gets a value indicating whether the "Move Start" action is disabled.
    /// </summary>
    public bool IsMoveStartDisabled => SelectionMode == ListGroupSelectionMode.Single
        ? SelectedItemEnd is null
        : SelectionMode == ListGroupSelectionMode.Multiple
            ? SelectedItemsEnd.IsNullOrEmpty()
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
    /// Enables the "Move All" Actions.
    /// </summary>
    [Parameter] public bool ShowMoveAll { get; set; } = true;

    /// <summary>
    /// Defines the color of the move buttons.
    /// </summary>
    [Parameter] public Color MoveButtonsColor { get; set; } = Color.Primary;

    /// <summary>
    /// Makes the list group scrollable by adding a vertical scrollbar.
    /// </summary>
    [Parameter] public bool Scrollable { get; set; } = true;

    /// <summary>
    /// Sets the TransferList MaxHeight. 
    /// Defaults to 300px.
    /// </summary>
    [Parameter] public string MaxHeight { get; set; } = "300px";

    /// <summary>
    /// Specifies the content to be rendered inside each item of the <see cref="ListView{TItem}"/>.
    /// </summary>
    [Parameter] public RenderFragment<ItemContext<TItem>> ItemStartTemplate { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside each item of the <see cref="ListView{TItem}"/>.
    /// </summary>
    [Parameter] public RenderFragment<ItemContext<TItem>> ItemEndTemplate { get; set; }

    /// <summary>
    /// Gets or sets the items in the list.
    /// </summary>
    [EditorRequired][Parameter] public List<TItem> Items { get; set; }

    /// <summary>
    /// Gets or sets the function to extract the value field from an item.
    /// </summary>
    [EditorRequired][Parameter] public Func<TItem, string> ValueField { get; set; }

    /// <summary>
    /// Gets or sets the function to extract the text field from an item.
    /// </summary>
    [EditorRequired][Parameter] public Func<TItem, string> TextField { get; set; }

    /// <summary>
    /// Whether the item may be moved to the Start List.
    /// </summary>
    [Parameter] public Func<TItem, bool> CanMoveToStart { get; set; }

    /// <summary>
    /// Whether the item may be moved to the End List.
    /// </summary>
    [Parameter] public Func<TItem, bool> CanMoveToEnd { get; set; }

    /// <summary>
    /// Gets or sets the items in the start list.
    /// </summary>
    [Parameter] public List<TItem> ItemsStart { get; set; }

    /// <summary>
    /// Gets or sets the event callback for changes in the start list.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> ItemsStartChanged { get; set; }

    /// <summary>
    /// Gets or sets the items in the end list.
    /// </summary>
    [Parameter] public List<TItem> ItemsEnd { get; set; }

    /// <summary>
    /// Gets or sets the event callback for changes in the end list.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> ItemsEndChanged { get; set; }

    /// <summary>
    /// Gets or sets item that is currently selected in the start list.
    /// </summary>
    [Parameter] public TItem SelectedItemStart { get; set; }

    /// <summary>
    /// Gets or sets the event callback for changes in the start list.
    /// </summary>
    [Parameter] public EventCallback<TItem> SelectedItemStartChanged { get; set; }

    /// <summary>
    /// Gets or sets item that is currently selected in the end list.
    /// </summary>
    [Parameter] public TItem SelectedItemEnd { get; set; }

    /// <summary>
    /// Gets or sets the event callback for changes in the end list.
    /// </summary>
    [Parameter] public EventCallback<TItem> SelectedItemEndChanged { get; set; }

    /// <summary>
    /// Gets or sets items that are currently selected in the start list.
    /// </summary>
    [Parameter] public List<TItem> SelectedItemsStart { get; set; }

    /// <summary>
    /// Gets or sets the event callback for changes to the items in the start list.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> SelectedItemsStartChanged { get; set; }

    /// <summary>
    /// Gets or sets items that are currently selected in the end list.
    /// </summary>
    [Parameter] public List<TItem> SelectedItemsEnd { get; set; }

    /// <summary>
    /// Gets or sets the event callback for changes to the items in the end list.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> SelectedItemsEndChanged { get; set; }

    #endregion
}
