#region Using directives
using System;
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

    private List<TValue> selectedValues;

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        selectedValues = SelectedValues?.ToList();
        base.OnInitialized();
    }

    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var selectedValuesChanged = parameters.TryGetValue<IReadOnlyList<TValue>>( nameof( SelectedValues ), out var paramSelectedValues ) && !paramSelectedValues.AreEqual( SelectedValues );

        await base.SetParametersAsync( parameters );

        if ( selectedValuesChanged )
            selectedValues = paramSelectedValues?.ToList();
    }

    protected Task HandleDropdownItemClicked( object value )
    {
        SelectedValue = Converters.ChangeType<TValue>( value );
        return SelectedValueChanged.InvokeAsync( SelectedValue );
    }

    protected Task HandleDropdownItemChecked( bool isChecked, TValue value )
    {
        selectedValues ??= new();

        if ( isChecked )
            selectedValues.Add( value );
        else
            selectedValues.Remove( value );

        return SelectedValuesChanged.InvokeAsync( selectedValues );
    }

    /// <summary>
    /// Sets focus on the input element, if it can be focused.
    /// </summary>
    /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Focus( bool scrollToElement = true )
    {
        return dropdownToggleRef.Focus( scrollToElement );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Whether the value is currently selected.
    /// </summary>
    protected bool IsSelected( TValue value )
        => selectedValues?.Any( x => x.Equals( value ) ) ?? false;

    /// <summary>
    /// Gets or sets the dropdown element id.
    /// </summary>
    [Parameter] public string ElementId { get; set; }

    /// <summary>
    /// Defines the color of toggle button.
    /// </summary>
    [Parameter] public Color Color { get; set; }

    /// <summary>
    /// If true, a dropdown menu will be right aligned.
    /// </summary>
    [Parameter] public bool RightAligned { get; set; }

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
    /// Method used to get the display field from the supplied data source.
    /// </summary>
    [Parameter] public Func<TItem, string> TextField { get; set; }

    /// <summary>
    /// Method used to get the value field from the supplied data source.
    /// </summary>
    [Parameter] public Func<TItem, TValue> ValueField { get; set; }

    /// <summary>
    /// Currently selected item value.
    /// </summary>
    [Parameter] public TValue SelectedValue { get; set; }

    /// <summary>
    /// Occurs after the selected value has changed.
    /// </summary>
    [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

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
    /// Currently selected item values.
    /// </summary>
    [Parameter] public IReadOnlyList<TValue> SelectedValues { get; set; }

    /// <summary>
    /// Occurs after the selected item values have changed.
    /// </summary>
    [Parameter] public EventCallback<IReadOnlyList<TValue>> SelectedValuesChanged { get; set; }

    #endregion
}