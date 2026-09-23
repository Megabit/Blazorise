#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Components.Autocomplete;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components.Internal;

/// <summary>
/// Internal renderer for a single Autocomplete suggestion.
/// </summary>
/// <typeparam name="TItem">Type of the suggested item.</typeparam>
/// <typeparam name="TValue">Type of the suggested value.</typeparam>
public partial class _AutocompleteItem<TItem, TValue> : ComponentBase
{
    #region Members

    private bool rendered;

    private bool shouldRender = true;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        var contextChanged = parameters.TryGetValue<ItemContext<TItem, TValue>>( nameof( Context ), out ItemContext<TItem, TValue> newContext ) && !AreEqual( Context, newContext );
        var elementIdChanged = parameters.TryGetValue<string>( nameof( ElementId ), out string newElementId ) && ElementId != newElementId;
        var classChanged = parameters.TryGetValue<string>( nameof( Class ), out string newClass ) && Class != newClass;
        var styleChanged = parameters.TryGetValue<string>( nameof( Style ), out string newStyle ) && Style != newStyle;
        var closeParentDropdownsChanged = parameters.TryGetValue<bool>( nameof( CloseParentDropdowns ), out bool newCloseParentDropdowns ) && CloseParentDropdowns != newCloseParentDropdowns;
        var highlightSearchChanged = parameters.TryGetValue<bool>( nameof( HighlightSearch ), out bool newHighlightSearch ) && HighlightSearch != newHighlightSearch;
        var searchChanged = parameters.TryGetValue<string>( nameof( Search ), out string newSearch ) && Search != newSearch;
        var itemContentChanged = parameters.TryGetValue<RenderFragment<ItemContext<TItem, TValue>>>( nameof( ItemContent ), out RenderFragment<ItemContext<TItem, TValue>> newItemContent ) && !Equals( ItemContent, newItemContent );

        shouldRender = shouldRender
            || !rendered
            || contextChanged
            || elementIdChanged
            || classChanged
            || styleChanged
            || closeParentDropdownsChanged
            || highlightSearchChanged
            || searchChanged
            || itemContentChanged;

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
        => shouldRender;

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        rendered = true;
        shouldRender = false;
        return base.OnAfterRenderAsync( firstRender );
    }

    private Task OnPointerDown()
        => PointerDown.InvokeAsync();

    private Task OnClicked( object value )
        => Context.Checkbox || !Selected.HasDelegate
            ? Task.CompletedTask
            : Selected.InvokeAsync( value );

    private Task OnCheckedChanged( bool _ )
        => Context.Checkbox && Selected.HasDelegate
            ? Selected.InvokeAsync( Context.Value )
            : Task.CompletedTask;

    private static bool AreEqual( ItemContext<TItem, TValue> first, ItemContext<TItem, TValue> second )
    {
        if ( ReferenceEquals( first, second ) )
            return true;

        if ( first is null || second is null )
            return false;

        return EqualityComparer<TItem>.Default.Equals( first.Item, second.Item )
            && EqualityComparer<TValue>.Default.Equals( first.Value, second.Value )
            && first.Text == second.Text
            && first.Index == second.Index
            && first.Active == second.Active
            && first.Focused == second.Focused
            && first.Disabled == second.Disabled
            && first.Checkbox == second.Checkbox;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the suggestion context.
    /// </summary>
    [Parameter] public ItemContext<TItem, TValue> Context { get; set; }

    /// <summary>
    /// Gets or sets the rendered suggestion element identifier.
    /// </summary>
    [Parameter] public string ElementId { get; set; }

    /// <summary>
    /// Gets or sets the rendered suggestion classes.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Gets or sets the rendered suggestion styles.
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// Gets or sets whether selecting the suggestion closes parent dropdowns.
    /// </summary>
    [Parameter] public bool CloseParentDropdowns { get; set; }

    /// <summary>
    /// Gets or sets whether matching search text is highlighted.
    /// </summary>
    [Parameter] public bool HighlightSearch { get; set; }

    /// <summary>
    /// Gets or sets the current search text.
    /// </summary>
    [Parameter] public string Search { get; set; }

    /// <summary>
    /// Gets or sets the custom suggestion content.
    /// </summary>
    [Parameter] public RenderFragment<ItemContext<TItem, TValue>> ItemContent { get; set; }

    /// <summary>
    /// Gets or sets the pointer-down callback.
    /// </summary>
    [Parameter] public EventCallback PointerDown { get; set; }

    /// <summary>
    /// Gets or sets the selection callback.
    /// </summary>
    [Parameter] public EventCallback<object> Selected { get; set; }

    #endregion
}