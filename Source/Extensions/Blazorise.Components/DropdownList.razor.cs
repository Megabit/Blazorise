#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components;

/// <summary>
/// A component that dynamically generated dropdown menu based on the supplied data-source.
/// </summary>
/// <typeparam name="TItem">Type of an item filtered by the component.</typeparam>
/// <typeparam name="TValue">Type of an SelectedValue field.</typeparam>
public partial class DropdownList<TItem, TValue> : ComponentBase
{
    #region Members

    /// <summary>
    /// Reference to the Dropdown component.
    /// </summary>
    protected Dropdown dropdownRef;

    /// <summary>
    /// Reference to the DropdownToggle component.
    /// </summary>
    protected DropdownToggle dropdownToggleRef;

    /// <summary>
    /// Determines if the filter is dirty and needs to be updated.
    /// </summary>
    private bool dirtyFilter = true;

    /// <summary>
    /// The filtered data based on the current filter text.
    /// </summary>
    private List<TItem> filteredData;

    #endregion

    #region Methods

    /// <summary>
    /// Handles the selected value change event.
    /// </summary>
    /// <param name="value">The new selected value.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task HandleDropdownItemClicked( object value )
    {
        if ( Multiple )
            return Task.CompletedTask;

        return ValueChanged.InvokeAsync( Converters.ChangeType<TValue>( value ) );
    }

    protected Task HandleDropdownItemChecked( bool isChecked, object fieldValue )
    {
        if ( !Multiple )
            return Task.CompletedTask;

        List<object> selectedValues;

        if ( Value is IEnumerable<TValue> values )
            selectedValues = values.Select( x => (object)x ).ToList();
        else if ( Value is IEnumerable objects && Value is not string )
            selectedValues = objects.Cast<object>().ToList();
        else
            selectedValues = new List<object>();

        if ( isChecked )
            selectedValues.Add( fieldValue );
        else
            selectedValues.Remove( fieldValue );

        return ValueChanged.InvokeAsync( Converters.ConvertListToReadOnlyList<TValue>( selectedValues ) );
    }

    /// <summary>
    /// Sets focus on the input element, if it can be focused.
    /// </summary>
    /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Focus( bool scrollToElement = true )
    {
        return dropdownToggleRef.Focus( scrollToElement );
    }

    private string GetItemText( TItem item )
    {
        if ( TextField is null )
            return string.Empty;

        return TextField.Invoke( item );
    }

    private object GetItemValue( TItem item )
    {
        if ( ValueField is null )
            return default;

        return ValueField.Invoke( item );
    }

    private bool GetItemDisabled( TItem item )
    {
        if ( DisabledItem is null )
            return false;

        return DisabledItem.Invoke( item );
    }

    private void FilterData( IQueryable<TItem> query )
    {
        dirtyFilter = false;

        if ( !Filterable || string.IsNullOrEmpty( FilterText ) )
        {
            filteredData = Data?.ToList();
            return;
        }

        if ( query == null )
        {
            filteredData = new List<TItem>();
            return;
        }

        if ( TextField == null )
            return;

        filteredData = Data.Where( x => TextField.Invoke( x ).Contains( FilterText, StringComparison.OrdinalIgnoreCase ) ).ToList();
    }

    private Task OnFilterTextChangedHandler( string filteredText )
    {
        FilterText = filteredText;
        dirtyFilter = true;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Check if the internal value is same as the new value.
    /// </summary>
    /// <param name="value">Value to check against the internal value.</param>
    /// <returns>True if the internal value matched the supplied value.</returns>
    protected bool IsSameAsInternalValue( TValue value )
    {
        if ( value is IEnumerable<TValue> values1 && Value is IEnumerable<TValue> values2 )
        {
            return values1.AreEqual( values2 );
        }
        else if ( value is IEnumerable objects1 && Value is IEnumerable objects2 )
        {
            return objects1.AreEqual( objects2 );
        }

        return value.IsEqual( Value );
    }

    /// <summary>
    /// Whether the value is currently selected.
    /// </summary>
    protected bool IsSelected( object value )
    {
        if ( Value is null )
            return false;

        if ( Value is IEnumerable<TValue> values )
        {
            return values.Any( x => x.IsEqual( value ) );
        }
        else if ( Value is IEnumerable objects && Value is not string )
        {
            return objects.Cast<object>().Any( x => x.IsEqual( value ) );
        }

        return Value.IsEqual( value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the multiple items can be selected.
    /// </summary>
    protected bool Multiple => SelectionMode == DropdownListSelectionMode.Checkbox;

    /// <summary>
    /// Gets or sets the filter text.
    /// </summary>
    private string FilterText { get; set; }

    /// <summary>
    /// Gets or sets the dropdown element id.
    /// </summary>
    [Parameter] public string ElementId { get; set; }

    /// <summary>
    /// Defines the color of toggle button.
    /// </summary>
    [Parameter] public Color Color { get; set; }

    /// <summary>
    /// Defines the size of toggle button.
    /// </summary>
    [Parameter] public Size DropdownToggleSize { get; set; }

    /// <summary>
    /// If true, a dropdown menu will be aligned to the end.
    /// </summary>
    [Parameter] public bool EndAligned { get; set; }

    /// <summary>
    /// If true, dropdown would not react to button click.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Dropdown-menu slide direction.
    /// </summary>
    [Parameter] public Direction Direction { get; set; }

    /// <summary>
    /// Gets or sets the DropdownList data-source.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Gets the filtered data based on the current filter text.
    /// </summary>
    private List<TItem> FilteredData
    {
        get
        {
            if ( dirtyFilter )
                FilterData( Data?.AsQueryable() );

            return filteredData;
        }
    }

    /// <summary>
    /// Method used to get the display field from the supplied data source.
    /// </summary>
    [Parameter] public Func<TItem, string> TextField { get; set; }

    /// <summary>
    /// Method used to get the value field from the supplied data source.
    /// </summary>
    [Parameter] public Func<TItem, object> ValueField { get; set; }

    /// <summary>
    /// Currently selected item value.
    /// </summary>
    [Parameter] public TValue Value { get; set; }

    /// <summary>
    /// Occurs after the selected value has changed.
    /// </summary>
    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// Enebles filter text input on the top of the items list.
    /// </summary>
    [Parameter] public bool Filterable { get; set; }

    /// <summary>
    /// Custom classname for dropdown element.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Custom styles for dropdown element.
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
    /// </summary>
    [Parameter] public int? TabIndex { get; set; }

    /// <summary>
    /// Sets the maximum height of the dropdown menu.
    /// </summary>
    [Parameter] public string MaxMenuHeight { get; set; }

    /// <summary>
    /// Gets or sets whether the dropdown will use the Virtualize functionality.
    /// </summary>
    [Parameter] public bool Virtualize { get; set; }

    /// <summary>
    /// Captures all the custom attribute that are not part of Blazorise component.
    /// </summary>
    [Parameter( CaptureUnmatchedValues = true )]
    public Dictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="DropdownList{TItem, TValue}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DropdownList{TItem, TValue}"/> Selection Mode.
    /// </summary>
    [Parameter] public DropdownListSelectionMode SelectionMode { get; set; } = DropdownListSelectionMode.Default;

    /// <summary>
    /// Method used to get the disabled items from the supplied data source.
    /// </summary>
    [Parameter] public Func<TItem, bool> DisabledItem { get; set; }

    #endregion
}