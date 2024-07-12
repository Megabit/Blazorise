#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Components.ListView;
using Blazorise.Extensions;
using Blazorise.Licensing;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Dynamically builds ListView component and it's items based in the supplied data.
/// </summary>
public partial class ListView<TItem> : ComponentBase
{
    #region Methods

    private IEnumerable<TItem> GetData()
    {
        var maxRowsLimit = LicenseChecker.GetListViewRowsLimit();

        if ( maxRowsLimit.HasValue )
        {
            return Data?.Take( maxRowsLimit.Value );
        }

        return Data;
    }

    private string GetItemText( TItem item )
    {
        if ( item is null || TextField is null )
            return string.Empty;

        return TextField.Invoke( item );
    }

    private string GetItemValue( TItem item )
    {
        if ( item is null || ValueField is null )
            return string.Empty;

        return ValueField.Invoke( item );
    }

    private bool GetItemDisabled( TItem item )
    {
        if ( item is null || DisabledItem is null )
            return false;

        return DisabledItem.Invoke( item );
    }

    private TextColor GetItemTextColor( TItem item )
    {
        if ( item is null || ItemTextColor is null )
            // is this correct to do?
            return TextColor.Default;

        return ItemTextColor.Invoke( item );
    }

    private Background GetItemBackground( TItem item )
    {
        if ( item is null || ItemBackground is null )
            // im assuming we should do default here, as no other value would make sense
            return Background.Default;

        return ItemBackground.Invoke( item );
    }

    private IFluentSpacing GetItemPadding( TItem item )
    {
        if ( item is null || ItemPadding is null )
            return null;

        return ItemPadding.Invoke( item );
    }

    private IFluentSpacing GetItemMargin( TItem item )
    {
        if ( item is null || ItemMargin is null )
            return null;

        return ItemMargin.Invoke( item );
    }

    private string GetItemClass( TItem item )
    {
        if ( item is null || ItemClass is null )
            return null;

        return ItemClass.Invoke( item );
    }

    private string GetItemStyle( TItem item )
    {
        if ( item is null || ItemStyle is null )
            return null;

        return ItemStyle.Invoke( item );
    }

    private List<string> GetItemValues( List<TItem> selectedItems )
    {
        if ( selectedItems is null || ValueField is null )
            return new List<string>();

        return selectedItems.Select( x => ValueField.Invoke( x ) ).ToList();
    }

    protected Task SelectedListGroupItemChanged( string value )
    {
        SelectedItem = GetItemBySelectedValue( value );

        return SelectedItemChanged.InvokeAsync( SelectedItem );
    }

    protected Task SelectedListGroupItemsChanged( List<string> values )
    {
        SelectedItems = GetItemsBySelectedValues( values );

        return SelectedItemsChanged.InvokeAsync( SelectedItems );
    }

    private TItem GetItemBySelectedValue( string selectedValue )
    {
        if ( !Data.IsNullOrEmpty() && ValueField is not null )
        {
            return Data.FirstOrDefault( x => ValueField.Invoke( x ) == selectedValue );
        }

        return default;
    }

    private List<TItem> GetItemsBySelectedValues( List<string> selectedValues )
    {
        if ( !Data.IsNullOrEmpty() && ValueField is not null )
        {
            return Data.Where( x => selectedValues.Contains( ValueField.Invoke( x ) ) ).ToList();
        }

        return default;
    }

    protected string ListGroupClassNames
        => Class;

    protected string ListGroupStyleNames
    {
        get
        {
            var sb = new StringBuilder();

            if ( !string.IsNullOrWhiteSpace( Height ) )
                sb.Append( $"height:{Height};" );
            if ( !string.IsNullOrWhiteSpace( MaxHeight ) )
                sb.Append( $"max-height:{MaxHeight};" );
            if ( !string.IsNullOrWhiteSpace( Style ) )
                sb.Append( Style );

            return sb.ToString();
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the license checker for the user session.
    /// </summary>
    [Inject] internal BlazoriseLicenseChecker LicenseChecker { get; set; }

    /// <summary>
    /// Defines the list-group behavior mode.
    /// </summary>
    [Parameter] public ListGroupMode Mode { get; set; }

    /// <summary>
    /// Defines the list-group selection mode.
    /// </summary>
    [Parameter] public ListGroupSelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Remove some borders and rounded corners to render list group items edge-to-edge in a parent container (e.g., cards).
    /// </summary>
    [Parameter] public bool Flush { get; set; }

    /// <summary>
    /// Makes the list group scrollable by adding a vertical scrollbar.
    /// </summary>
    [Parameter] public bool Scrollable { get; set; } = true;

    /// <summary>
    /// Gets or sets the items data-source.
    /// </summary>
    [EditorRequired]
    [Parameter] public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Method used to get the display field from the supplied data source.
    /// </summary>
    [EditorRequired]
    [Parameter] public Func<TItem, string> TextField { get; set; }

    /// <summary>
    /// Method used to get the value field from the supplied data source.
    /// </summary>
    [EditorRequired]
    [Parameter] public Func<TItem, string> ValueField { get; set; }

    /// <summary>
    /// Method used to get the disabled items from the supplied data source.
    /// </summary>
    [Parameter] public Func<TItem, bool> DisabledItem { get; set; }

    /// <summary>
    /// Method used to get the text color for the item.
    /// </summary>
    [Parameter] public Func<TItem, TextColor> ItemTextColor { get; set; }

    /// <summary>
    /// Method used to get the background color for the item.
    /// </summary>
    [Parameter] public Func<TItem, Background> ItemBackground { get; set; }

    /// <summary>
    /// Method used to get the padding for the item.
    /// </summary>
    [Parameter] public Func<TItem, IFluentSpacing> ItemPadding { get; set; }

    /// <summary>
    /// Method used to get the margin for the item.
    /// </summary>
    [Parameter] public Func<TItem, IFluentSpacing> ItemMargin { get; set; }

    /// <summary>
    /// Method used to get the class for the item.
    /// </summary>
    [Parameter] public Func<TItem, string> ItemClass { get; set; }

    /// <summary>
    /// Method used to get the style for the item.
    /// </summary>
    [Parameter] public Func<TItem, string> ItemStyle { get; set; }

    /// <summary>
    /// Currently selected item.
    /// </summary>
    [Parameter] public TItem SelectedItem { get; set; }

    /// <summary>
    /// Currently selected items.
    /// </summary>
    [Parameter] public List<TItem> SelectedItems { get; set; }

    /// <summary>
    /// Occurs after the selected item has changed.
    /// </summary>
    [Parameter] public EventCallback<TItem> SelectedItemChanged { get; set; }

    /// <summary>
    /// Occurs after the selected items has changed.
    /// </summary>
    [Parameter] public EventCallback<List<TItem>> SelectedItemsChanged { get; set; }

    /// <summary>
    /// Custom css class-names.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Custom styles.
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// Sets the ListView Height. 
    /// Defaults to empty.
    /// </summary>
    [Parameter] public string Height { get; set; }

    /// <summary>
    /// Sets the ListView MaxHeight. 
    /// Defaults to 250px.
    /// </summary>
    [Parameter] public string MaxHeight { get; set; } = "250px";

    /// <summary>
    /// Specifies the content to be rendered inside each item of the <see cref="ListView{TItem}"/>.
    /// </summary>
    [Parameter] public RenderFragment<ItemContext<TItem>> ItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets whether the listview will use the Virtualize functionality.
    /// </summary>
    [Parameter] public bool Virtualize { get; set; }

    /// <summary>
    /// Captures all the custom attribute that are not part of Blazorise component.
    /// </summary>
    [Parameter( CaptureUnmatchedValues = true )]
    public Dictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ListView{TItem}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}