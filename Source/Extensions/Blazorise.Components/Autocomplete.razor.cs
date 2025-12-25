#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Components.Autocomplete;
using Blazorise.Extensions;
using Blazorise.Licensing;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
#endregion

namespace Blazorise.Components;

/// <summary>
/// The autocomplete is a normal text input enhanced by a panel of suggested options.
/// </summary>
/// <typeparam name="TItem">Type of an item filtered by the autocomplete component.</typeparam>
/// <typeparam name="TValue">Type of an SelectedValue field.</typeparam>
public partial class Autocomplete<TItem, TValue> : BaseInputComponent<TValue>, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Element reference to the Autocomplete's inner virtualize.
    /// </summary>
    private Virtualize<TItem> virtualizeRef;

    /// <summary>
    /// Gets the CancellationTokenSource which could be used to issue a cancellation.
    /// </summary>
    private CancellationTokenSource cancellationTokenSource;

    /// <summary>
    /// Reference to the TextInput component.
    /// </summary>
    private TextInput textInputRef;

    /// <summary>
    /// Original data-source.
    /// </summary>
    private IEnumerable<TItem> data;

    /// <summary>
    /// Holds the filtered data based on the filter.
    /// </summary>
    private List<TItem> filteredData = new();

    /// <summary>
    /// Marks the autocomplete to reload entire data source based on the current filter settings.
    /// </summary>
    private bool dirtyFilter;

    /// <summary>
    /// Allow dropdown visibility
    /// </summary>
    private bool canShowDropDown;

    private string currentSearch;
    private string currentSearchParam;
    private bool selectedValueParamChanged;
    private bool selectedTextParamChanged;
    private bool selectedValuesParamChanged;
    private bool selectedTextsParamChanged;
    private bool dataParamChanged;

    private TValue selectedValueParam;
    private bool selectedValueParamDefined;

    private List<TValue> selectedValuesParam;
    private List<string> selectedTextsParam;
    private bool autocompleteAutofocus;

    /// <summary>
    /// Captured parameter snapshots for safe access after awaited operations.
    /// </summary>
    /// <summary>
    /// Captured SelectionMode parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<AutocompleteSelectionMode> paramSelectionMode;

    /// <summary>
    /// Captured Search parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<string> paramSearch;

    /// <summary>
    /// Captured SelectedValue parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<TValue> paramSelectedValue;

    /// <summary>
    /// Captured SelectedText parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<string> paramSelectedText;

    /// <summary>
    /// Captured SelectedValues parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<IEnumerable<TValue>> paramSelectedValues;

    /// <summary>
    /// Captured SelectedTexts parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<IEnumerable<string>> paramSelectedTexts;

    /// <summary>
    /// Captured Data parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<IEnumerable<TItem>> paramData;

    /// <summary>
    /// Captured SelectedValueExpression parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<Expression<Func<TValue>>> paramSelectedValueExpression;

    /// <summary>
    /// Captured SelectedValuesExpression parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<Expression<Func<List<TValue>>>> paramSelectedValuesExpression;

    /// <summary>
    /// Workaround for the issue where the dropdown closes when clicking on the checkbox
    /// </summary>
    private bool clickFromCheck;
    /// <summary>
    /// Workaround for the issue where the dropdown closes when clicking on the checkbox
    /// </summary>
    private bool focusFromCheck;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( nameof( SelectionMode ), SelectionMode, out paramSelectionMode );
        parameters.TryGetParameter( nameof( Search ), Search, out paramSearch );
        parameters.TryGetParameter( nameof( SelectedValue ), SelectedValue, out paramSelectedValue );
        parameters.TryGetParameter( nameof( SelectedText ), SelectedText, out paramSelectedText );
        parameters.TryGetParameter( nameof( SelectedValues ), SelectedValues, value => selectedValuesParam.AreEqualOrdered( value ), out paramSelectedValues );
        parameters.TryGetParameter( nameof( SelectedTexts ), SelectedTexts, value => selectedTextsParam.AreEqualOrdered( value ), out paramSelectedTexts );
        parameters.TryGetParameter( nameof( Data ), Data, value => data.IsEqual( value ), out paramData );
        parameters.TryGetParameter( nameof( SelectedValueExpression ), SelectedValueExpression, out paramSelectedValueExpression );
        parameters.TryGetParameter( nameof( SelectedValuesExpression ), SelectedValuesExpression, out paramSelectedValuesExpression );
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        await base.OnBeforeSetParametersAsync( parameters );

        AutocompleteSelectionMode selectionMode = paramSelectionMode.Defined
            ? paramSelectionMode.Value
            : SelectionMode;

        bool isMultiple = selectionMode == AutocompleteSelectionMode.Multiple || selectionMode == AutocompleteSelectionMode.Checkbox;

        if ( paramSearch.Defined && currentSearchParam != paramSearch.Value )
        {
            currentSearch = null;
        }

        bool hasSelectedValueParam = paramSelectedValue.Defined;

        if ( hasSelectedValueParam )
        {
            selectedValueParamChanged = !selectedValueParamDefined || !paramSelectedValue.Value.IsEqual( selectedValueParam );
            selectedValueParam = paramSelectedValue.Value;
            selectedValueParamDefined = true;
        }
        else
        {
            selectedValueParamChanged = false;
            selectedValueParamDefined = false;
        }

        if ( !paramValue.Defined && hasSelectedValueParam )
        {
            paramValue = new ComponentParameterInfo<TValue>( paramSelectedValue.Value, true, selectedValueParamChanged );

            if ( Rendered && paramValue.Changed )
            {
                ExecuteAfterRender( Revalidate );
            }
        }

        if ( paramValue.Defined && !hasSelectedValueParam )
        {
            selectedValueParamChanged = paramValue.Changed;
        }

        selectedTextParamChanged = paramSelectedText.Defined && !SelectedText.IsEqual( paramSelectedText.Value );

        selectedValuesParamChanged = paramSelectedValues.Defined && paramSelectedValues.Changed;

        selectedTextsParamChanged = paramSelectedTexts.Defined && paramSelectedTexts.Changed;

        dataParamChanged = paramData.Defined && paramData.Changed;

        if ( isMultiple && Rendered && ( selectedValuesParamChanged || selectedTextsParamChanged ) )
        {
            ExecuteAfterRender( Revalidate );
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        if ( !paramValueExpression.Defined && paramSelectedValueExpression.Defined )
        {
            ValueExpression = paramSelectedValueExpression.Value;
        }

        if ( ParentValidation is not null )
        {
            if ( IsMultiple )
            {
                if ( paramSelectedValuesExpression.Defined )
                {
                    await ParentValidation.InitializeInputExpression( paramSelectedValuesExpression.Value );
                }
            }
            else
            {
                if ( paramValueExpression.Defined )
                {
                    await ParentValidation.InitializeInputExpression( paramValueExpression.Value );
                }
                else if ( paramSelectedValueExpression.Defined )
                {
                    await ParentValidation.InitializeInputExpression( paramSelectedValueExpression.Value );
                }
            }

            await InitializeValidation();
        }

        if ( paramAutofocus.Defined && autocompleteAutofocus != paramAutofocus.Value )
        {
            autocompleteAutofocus = paramAutofocus.Value;

            if ( autocompleteAutofocus )
            {
                if ( ParentFocusableContainer is not null )
                {
                    ParentFocusableContainer.NotifyFocusableComponentInitialized( this );
                }
                else
                {
                    ExecuteAfterRender( () => Focus() );
                }
            }
            else
            {
                ParentFocusableContainer?.NotifyFocusableComponentRemoved( this );
            }
        }

        await SynchronizeSingle( selectedValueParamChanged, selectedTextParamChanged );
        await SynchronizeMultiple( selectedValuesParamChanged, selectedTextsParamChanged, dataParamChanged );
    }

    protected async ValueTask<ItemsProviderResult<TItem>> VirtualizeItemsProviderHandler( ItemsProviderRequest request )
    {
        // Credit to Steve Sanderson's Quickgrid implementation
        // Debounce the requests. This eliminates a lot of redundant queries at the cost of slight lag after interactions.
        // TODO: Consider making this configurable, or smarter (e.g., doesn't delay on first call in a batch, then the amount
        // of delay increases if you rapidly issue repeated requests, such as when scrolling a long way)
        await Task.Delay( 100 );

        if ( request.CancellationToken.IsCancellationRequested )
            return default;

        await HandleVirtualizeReadData( request.StartIndex, request.Count, request.CancellationToken );

        if ( request.CancellationToken.IsCancellationRequested )
            return default;
        else
            return new( Data.ToList(), TotalItems ?? default );
    }

    private async Task SynchronizeSingle( bool selectedValueParamChanged, bool selectedTextParamChanged )
    {
        if ( selectedTextParamChanged && !selectedValueParamChanged )
        {
            if ( !string.IsNullOrEmpty( SelectedText ) )
            {
                var item = GetItemByText( SelectedText );
                if ( item is null )
                {
                    if ( !FreeTyping )
                    {
                        await ResetSelectedText();
                    }
                    SelectedValue = default;
                    await SelectedValueChanged.InvokeAsync( SelectedValue );
                }
                else
                {
                    var value = GetItemValue( item );
                    if ( !SelectedValue.IsEqual( value ) )
                    {
                        SelectedValue = value;
                        await SelectedValueChanged.InvokeAsync( value );
                    }
                }
            }

            if ( !IsMultiple && Search != SelectedText )
            {
                currentSearch = SelectedText;
                DirtyFilter();

                await Task.WhenAll(
                    ResetSelectedValue(),
                    InvokeSearchChanged( currentSearch )
                );
            }
        }

        if ( selectedValueParamChanged )
        {
            var item = GetItemByValue( SelectedValue );
            if ( item is null )
            {
                await ResetSelectedValue();
            }
            else
            {
                string text = GetItemText( item );
                var textAlsoChangedAndMatchesSelection = ( selectedTextParamChanged && text == SelectedText );
                if ( text != SelectedText || textAlsoChangedAndMatchesSelection )
                {
                    SelectedText = text;
                    await SelectedTextChanged.InvokeAsync( SelectedText );

                    if ( !IsMultiple && Search != SelectedText )
                    {
                        currentSearch = SelectedText;
                        DirtyFilter();

                        await InvokeSearchChanged( currentSearch );
                    }
                }
            }
        }
    }

    private async Task SynchronizeMultiple( bool selectedValuesParamChanged, bool selectedTextsParamChanged, bool dataParamChanged )
    {
        List<TValue> values = null;
        List<string> texts = null;

        if ( selectedTextsParamChanged && selectedTextsParam is not null && !Data.IsNullOrEmpty() && !selectedValuesParamChanged )
        {
            values = Data.IntersectBy( SelectedTexts, e => GetItemText( e ) ).Select( e => GetItemValue( e ) ).ToList();
            if ( !FreeTyping )
            {
                texts = Data.Select( e => GetItemText( e ) ).Intersect( SelectedTexts ).ToList();
            }
        }

        var shouldSyncTextsFromValues = selectedValuesParamChanged || ( dataParamChanged && SelectedTexts.IsNullOrEmpty() );

        if ( shouldSyncTextsFromValues && selectedValuesParam is not null && !Data.IsNullOrEmpty() )
        {
            texts = Data.IntersectBy( SelectedValues, e => GetItemValue( e ) ).Select( e => GetItemText( e ) ).ToList();
        }

        if ( values is not null && !SelectedValues.AreEqualOrdered( values ) )
        {
            SelectedValues = values;
            await SelectedValuesChanged.InvokeAsync( values );
        }

        if ( texts is not null && !SelectedTexts.AreEqualOrdered( texts ) )
        {
            SelectedTexts = texts;
            await SelectedTextsChanged.InvokeAsync( texts );
        }

        if ( ( selectedValuesParamChanged && values is null && SelectedValues is null ) || ( selectedTextsParamChanged && texts is null && SelectedTexts is null ) )
        {
            await Task.WhenAll( ResetSelectedValues(), ResetSelectedTexts() );
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        InputElementId = IdGenerator.Generate;

        ExecuteAfterRender( async () =>
        {
            ElementRef = textInputRef.ElementRef;
            await JSClosableModule.RegisterLight( ElementRef );
        } );

        if ( ManualReadMode )
            await Reload();

        if ( AutoSelectFirstItem && !IsMultiple )
        {
            if ( HasFilteredData )
            {
                var item = FilteredData.First();
                currentSearch = SelectedText = GetItemText( item );
                var value = GetItemValue( item );
                SelectedValue = value;

                await Task.WhenAll(
                    InvokeSearchChanged( currentSearch ),
                    SelectedTextChanged.InvokeAsync( SelectedText ),
                    SelectedValueChanged.InvokeAsync( value )
                );
            }
        }

        await base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( Converters.TryChangeType<TValue>( value, out var result ) )
            return Task.FromResult( new ParseValue<TValue>( true, result, null ) );

        return Task.FromResult( ParseValue<TValue>.Empty );
    }

    /// <summary>
    /// Handles the search field onchange or oninput event.
    /// </summary>
    /// <param name="text">Search value.</param>
    /// <returns>Returns awaitable task</returns>
    protected async Task OnTextChangedHandler( string text )
    {
        DirtyFilter();

        //If input field is empty, clear current SelectedValue.
        if ( string.IsNullOrEmpty( text ) )
        {
            await ResetCurrentSearch();
            await ResetSelected();

            if ( ManualReadMode )
                await Reload();
        }
        else
        {
            await SetCurrentSearch( text );

            if ( ManualReadMode )
                await Reload();

            if ( !HasFilteredData )
            {
                await ResetActiveItemIndex();
            }

            if ( FreeTyping )
            {
                SelectedText = Search;
                await SelectedTextChanged.InvokeAsync( SelectedText );
            }
        }

        await OpenDropdown();

        if ( !HasFilteredData )
        {
            if ( !string.IsNullOrEmpty( Search ) )
            {
                await NotFound.InvokeAsync( Search );
            }
        }

        await SearchTextChanged.InvokeAsync( text );
        await Revalidate();
    }

    /// <summary>
    /// Scrolls an item into view.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public async Task ScrollItemIntoView( int index )
    {
        if ( DropdownVisible && index >= 0 )
        {
            await JSUtilitiesModule.ScrollElementIntoView( DropdownItemId( index ) );
        }
    }

    /// <summary>
    /// Handles the search field OnKeyDown event.
    /// </summary>
    /// <param name="eventArgs">Event arguments.</param>
    /// <returns>Returns awaitable task</returns>
    protected async Task OnTextKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        await OnKeyDownHandler( eventArgs );

        if ( eventArgs.Code == "Escape" )
        {
            await Close();
            await SearchKeyDown.InvokeAsync( eventArgs );
            return;
        }

        if ( IsMultiple && string.IsNullOrEmpty( Search ) && eventArgs.Code == "Backspace" )
        {
            await RemoveMultipleTextAndValue( SelectedTexts?.LastOrDefault() );
            await SearchKeyDown.InvokeAsync( eventArgs );
            return;
        }

        if ( IsConfirmKey( eventArgs ) )
        {
            if ( IsMultiple && FreeTyping && !string.IsNullOrEmpty( Search ) && ActiveItemIndex < 0 )
            {
                await AddMultipleText( Search );
                if ( CloseOnSelection )
                {
                    await ResetCurrentSearch();
                    await Close();
                }
                await SearchKeyDown.InvokeAsync( eventArgs );
                return;
            }

            if ( SelectionMode == AutocompleteSelectionMode.Checkbox )
            {
                await SearchKeyDown.InvokeAsync( eventArgs );
                return;
            }

            await SelectedOrResetOnCommit();
            await SearchKeyDown.InvokeAsync( eventArgs );
            return;
        }

        if ( !DropdownVisible )
        {
            await OpenDropdown();
            await SearchKeyDown.InvokeAsync( eventArgs );
            return;
        }

        if ( eventArgs.Code == "ArrowUp" )
        {
            await UpdateActiveFilterIndex( ActiveItemIndex - 1 );
        }
        else if ( eventArgs.Code == "ArrowDown" )
        {
            await UpdateActiveFilterIndex( ActiveItemIndex + 1 );
        }

        await ScrollItemIntoView( Math.Max( 0, ActiveItemIndex ) );
        await SearchKeyDown.InvokeAsync( eventArgs );
    }

    private Task OnTextKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        return OnKeyPressHandler( eventArgs );
    }

    private Task OnTextKeyUpHandler( KeyboardEventArgs eventArgs )
    {
        return OnKeyUpHandler( eventArgs );
    }

    private Task OnTextFocusInHandler( FocusEventArgs eventArgs )
    {
        return OnFocusInHandler( eventArgs );
    }

    private Task OnTextFocusOutHandler( FocusEventArgs eventArgs )
    {
        return OnFocusOutHandler( eventArgs );
    }

    /// <summary>
    /// Handles the search field onfocusin event.
    /// </summary>
    /// <param name="eventArgs">Event arguments.</param>
    /// <returns>Returns awaitable task</returns>
    protected async Task OnTextFocusHandler( FocusEventArgs eventArgs )
    {
        if ( focusFromCheck )
        {
            focusFromCheck = false;
            return;
        }

        TextFocused = true;
        if ( ManualReadMode || MinLength <= 0 )
            await Reload();

        await OpenDropdown();
        await OnFocusHandler( eventArgs );
        await SearchFocus.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handles the search field onblur event.
    /// </summary>
    /// <param name="eventArgs">Event arguments.</param>
    /// <returns>Returns awaitable task</returns>
    protected async Task OnTextBlurHandler( FocusEventArgs eventArgs )
    {
        if ( SelectionMode == AutocompleteSelectionMode.Checkbox && clickFromCheck )
        {
            // Only defer when blur originated from a checkbox click
            ExecuteAfterRender( HandleBlurHandler );
        }
        else
        {
            await HandleBlurHandler();
        }

        async Task HandleBlurHandler()
        {
            if ( clickFromCheck )
            {
                clickFromCheck = false;
                focusFromCheck = true;
                await textInputRef.Focus();
                return;
            }
            await Close();

            if ( IsMultiple )
            {
                await ResetSelected();
                await ResetCurrentSearch();
            }
            else
            {
                if ( !FreeTyping && string.IsNullOrEmpty( SelectedText ) )
                {
                    await ResetSelected();
                    await ResetCurrentSearch();
                    return;
                }

                await SelectedOrResetOnCommit();
            }

            TextFocused = false;

            await OnBlurHandler( eventArgs );
            await SearchBlur.InvokeAsync( eventArgs );
        }
    }

    private Task InvokeSearchChanged( string searchValue )
    {
        return SearchChanged.InvokeAsync( searchValue );
    }

    private async Task SelectedOrResetOnCommit()
    {
        if ( ActiveItemIndex >= 0 && DropdownVisible )
        {
            if ( FilteredData?.Count > 0 )
            {
                var item = FilteredData[ActiveItemIndex];
                if ( item != null && ValueField != null )
                {
                    await OnDropdownItemSelected( ValueField.Invoke( item ) );
                }
            }
        }
        else
        {
            var itemValues = GetItemValues( Search );
            if ( itemValues.IsNullOrEmpty() || !itemValues.Any( IsSelectedvalue ) )
            {
                if ( !FreeTyping )
                {
                    await ResetCurrentSearch();
                    await ResetSelectedText();
                }
                await ResetSelectedValue();
            }
        }
    }


    private async Task OnDropdownItemSelected( object value )
    {
        if ( Disabled || ReadOnly )
            return;

        clickFromCheck = ( SelectionMode == AutocompleteSelectionMode.Checkbox );

        if ( SelectionMode == AutocompleteSelectionMode.Default && !IsMultiple )
        {
            await Close();
        }

        if ( IsMultiple )
        {
            if ( CloseOnSelection )
            {
                await Close();
                await ResetCurrentSearch();
            }
            else if ( IsSuggestSelectedItems )
            {
                ActiveItemIndex = FilteredData.Index( x => ValueField( x ).IsEqual( value ) );
            }
        }

        var selectedTValue = Converters.ChangeType<TValue>( value );
        if ( IsSelectedvalue( selectedTValue ) )
        {
            if ( IsMultiple )
            {
                await RemoveMultipleTextAndValue( selectedTValue );
                if ( !IsSuggestSelectedItems )
                {
                    DirtyFilter();
                }

                await Revalidate();
            }
            else
            {
                await ResyncText();
            }

            return;
        }

        if ( IsMultiple )
        {
            await AddMultipleTextAndValue( selectedTValue );

            if ( !IsSuggestSelectedItems )
            {
                DirtyFilter();
            }
        }
        else
        {
            SelectedValue = selectedTValue;
            currentSearch = SelectedText = GetItemText( SelectedValue );
            DirtyFilter();

            await Task.WhenAll(
                SelectedValueChanged.InvokeAsync( SelectedValue ),
                InvokeSearchChanged( currentSearch ),
                SelectedTextChanged.InvokeAsync( SelectedText )
            );
        }

        ActiveItemIndex = Math.Max( 0, Math.Min( FilteredData.Count - 1, ActiveItemIndex ) );
        await Revalidate();
    }

    private async Task ResyncText()
    {
        var itemText = GetItemText( SelectedValue );
        if ( Search != itemText )
        {
            currentSearch = itemText;
            await InvokeSearchChanged( currentSearch );

            if ( SelectedText != itemText )
            {
                SelectedText = itemText;
                await SelectedTextChanged.InvokeAsync( SelectedText );
            }
        }
    }

    protected async Task HandleReadData( CancellationToken cancellationToken = default )
    {
        try
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );

            Loading = true;

            if ( !cancellationTokenSource.Token.IsCancellationRequested && IsTextSearchable )
            {
                await ReadData.InvokeAsync( new( Search, cancellationToken: cancellationTokenSource.Token ) );
                await Task.Yield(); // rebind Data after ReadData
            }
        }
        catch ( OperationCanceledException )
        {
            // Expected during rapid typing
        }
        finally
        {
            var wasCancelled = cancellationTokenSource?.IsCancellationRequested == true;

            Loading = false;

            if ( wasCancelled )
            {
                await InvokeAsync( () => Reload() );
            }
        }
    }

    protected async Task HandleVirtualizeReadData( int startIdx, int count, CancellationToken cancellationToken )
    {
        try
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );

            Loading = true;

            if ( !cancellationToken.IsCancellationRequested )
            {
                await ReadData.InvokeAsync( new( Search, startIdx, count, cancellationToken ) );
                await Task.Yield(); // rebind Data after ReadData
            }
        }
        finally
        {
            Loading = false;
        }
    }

    /// <summary>
    /// Triggers the reload of the <see cref="Autocomplete{TItem, TValue}"/> data.
    /// Makes sure not to reload if the <see cref="Autocomplete{TItem, TValue}"/> is in a loading state.
    /// </summary>
    /// <returns>Returns the awaitable task.</returns>
    public async Task Reload( CancellationToken cancellationToken = default )
    {
        if ( Loading )
        {
            cancellationTokenSource?.Cancel();
            return;
        }

        if ( VirtualizeManualReadMode )
        {
            if ( virtualizeRef is null )
                await InvokeAsync( () => HandleVirtualizeReadData( 0, 10, cancellationToken ) );
            else
                await virtualizeRef.RefreshDataAsync();
        }
        else if ( ManualReadMode )
        {
            await HandleReadData( cancellationToken );
        }

        DirtyFilter();
        await InvokeAsync( StateHasChanged );
    }

    private async Task ResetSelectedText()
    {
        var notifyChange = SelectedText is not null;

        SelectedText = null;
        if ( notifyChange )
            await SelectedTextChanged.InvokeAsync( SelectedText );
    }

    private async Task ResetSelectedValue()
    {
        var notifyChange = SelectedValue is not null;

        SelectedValue = default;
        if ( notifyChange )
            await SelectedValueChanged.InvokeAsync( SelectedValue );
    }

    private async Task ResetCurrentSearch()
    {
        currentSearch = string.Empty;

        await InvokeSearchChanged( currentSearch );
    }

    private async Task ResetSelectedValues()
    {
        SelectedValues?.Clear();
        await SelectedValuesChanged.InvokeAsync( SelectedValues );
    }

    private async Task ResetSelectedTexts()
    {
        SelectedTexts?.Clear();
        await SelectedTextsChanged.InvokeAsync( SelectedTexts );
    }

    private async Task SetCurrentSearch( string searchValue )
    {
        currentSearch = searchValue;

        await InvokeSearchChanged( currentSearch );
    }

    private Task ResetActiveItemIndex()
    {
        ActiveItemIndex = -1;
        return Task.CompletedTask;
    }

    private async Task AddMultipleValue( TValue value )
    {
        SelectedValues ??= new();

        if ( !SelectedValues.Contains( value ) && value != null )
        {
            SelectedValues.Add( value );
            await SelectedValuesChanged.InvokeAsync( SelectedValues );
        }
    }

    private Task AddMultipleText( TValue value )
        => AddMultipleText( GetItemText( value ) );

    private Task AddMultipleText( string text )
    {
        SelectedTexts ??= new();

        if ( !string.IsNullOrEmpty( text ) && !SelectedTexts.Contains( FreeTyping ? text.Trim() : text ) )
        {
            SelectedTexts.Add( text );
            return SelectedTextsChanged.InvokeAsync( SelectedTexts );
        }

        return Task.CompletedTask;
    }

    private async Task RemoveMultipleText( string text )
    {
        if ( SelectedTexts is null )
            return;

        var removed = SelectedTexts.Remove( text );

        if ( removed )
        {
            await SelectedTextsChanged.InvokeAsync( SelectedTexts );

            if ( SelectionMode == AutocompleteSelectionMode.Multiple )
                DirtyFilter();
        }
    }

    private async Task RemoveMultipleValue( TValue value )
    {
        if ( SelectedValues is null )
            return;

        var removed = SelectedValues.Remove( value );

        if ( removed )
            await SelectedValuesChanged.InvokeAsync( SelectedValues );
    }

    /// <summary>
    /// Adds a Multiple Selection.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task AddMultipleTextAndValue( TValue value )
    {
        await Task.WhenAll(
            AddMultipleText( value ),
            AddMultipleValue( value )
        );
    }

    /// <summary>
    /// Removes a Multiple Selection.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async Task RemoveMultipleTextAndValue( string text )
    {
        if ( Disabled || ReadOnly )
            return;

        var selectedValue = GetValueByText( text );

        await RemoveMultipleText( text );
        await RemoveMultipleValue( selectedValue );

        await Revalidate();

        DirtyFilter();
    }

    /// <summary>
    /// Removes a Multiple Selection.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task RemoveMultipleTextAndValue( TValue value )
    {
        var text = GetItemText( value );

        if ( string.IsNullOrEmpty( text ) && SelectedValues is not null && SelectedTexts is not null )
        {
            var selectedValueIndex = SelectedValues.IndexOf( value );

            if ( selectedValueIndex > -1 && selectedValueIndex < SelectedTexts.Count )
                text = SelectedTexts[selectedValueIndex];
        }

        await RemoveMultipleText( text );
        await RemoveMultipleValue( value );

        await Revalidate();

        DirtyFilter();
    }

    private void FilterData()
    {
        FilterData( Data?.AsQueryable() );
    }

    private void FilterData( IQueryable<TItem> query )
    {
        if ( query == null )
        {
            filteredData.Clear();
            return;
        }

        if ( TextField == null )
            return;

        if ( !ManualReadMode )
        {
            if ( IsMultiple && !IsSuggestSelectedItems && !SelectedValues.IsNullOrEmpty() )
                query = query.Where( x => !SelectedValues.Contains( ValueField.Invoke( x ) ) );

            if ( CustomFilter != null )
            {
                query = from q in query
                        where q != null
                        where CustomFilter( q, Search )
                        select q;
            }
            else if ( Filter == AutocompleteFilter.Contains )
            {
                query = from q in query
                        let text = GetItemText( q )
                        where text.IndexOf( Search, 0, StringComparison.OrdinalIgnoreCase ) >= 0
                        select q;
            }
            else
            {
                query = from q in query
                        let text = GetItemText( q )
                        where text.StartsWith( Search, StringComparison.OrdinalIgnoreCase )
                        select q;
            }
        }

        var maxRowsLimit = BlazoriseLicenseLimitsHelper.GetAutocompleteRowsLimit( LicenseChecker );

        if ( maxRowsLimit.HasValue )
        {
            filteredData = query.Take( maxRowsLimit.Value ).ToList();
        }
        else
        {
            filteredData = query.ToList();
        }

        dirtyFilter = false;
    }

    /// <summary>
    /// Clears the current selection.
    /// </summary>
    public async Task ResetSelected()
    {
        await ResetActiveItemIndex();
        await ResetSelectedText();
        await ResetSelectedValue();
    }

    /// <summary>
    /// Clears the selected value and the search field.
    /// </summary>
    public async Task Clear()
    {
        await ResetSelected();
        await ResetSelectedTexts();
        await ResetSelectedValues();
        await SetCurrentSearch( string.Empty );
    }

    private Task UpdateActiveFilterIndex( int activeItemIndex )
    {
        if ( FilteredData.Count == 0 )
        {
            ResetActiveItemIndex();
            return Task.CompletedTask;
        }

        ActiveItemIndex = Math.Max( 0, Math.Min( FilteredData.Count - 1, activeItemIndex ) );
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;

            if ( Rendered )
            {
                var task = JSClosableModule.UnregisterLight( ElementRef );

                try
                {
                    await task;
                }
                catch when ( task.IsCanceled )
                {
                }
                catch ( Microsoft.JSInterop.JSDisconnectedException )
                {
                }
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Sets focus on the input element, if it can be focused.
    /// </summary>
    /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override Task Focus( bool scrollToElement = true )
    {
        if ( textInputRef != null )
            return textInputRef.Focus( scrollToElement );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Closes the <see cref="Autocomplete{TItem, TValue}"/> Dropdown.
    /// </summary>
    /// <returns></returns>
    public Task Close()
    {
        return Close( CloseReason.UserClosing );
    }

    /// <summary>
    /// Closes the <see cref="Autocomplete{TItem, TValue}"/> Dropdown.
    /// </summary>
    /// <param name="closeReason">Specifies the reason for a component close event.</param>
    /// <returns></returns>
    public async Task Close( CloseReason closeReason )
    {
        canShowDropDown = false;
        await ResetActiveItemIndex();

        await Closed.InvokeAsync( new AutocompleteClosedEventArgs( closeReason ) );
    }

    /// <summary>
    /// Opens the <see cref="Autocomplete{TItem, TValue}"/> Dropdown.
    /// </summary>
    /// <returns></returns>
    private async Task Open()
    {
        var triggerOpened = !canShowDropDown;
        canShowDropDown = true;
        if ( triggerOpened )
            await Opened.InvokeAsync();
    }

    /// <summary>
    /// Opens the <see cref="Autocomplete{TItem, TValue}"/> Dropdown.
    /// </summary>
    /// <returns></returns>
    public async Task OpenDropdown()
    {
        await Open();
        if ( HasFilteredData && AutoPreSelect )
        {
            ActiveItemIndex = 0;
            ExecuteAfterRender( () => ScrollItemIntoView( ActiveItemIndex ) );
        }
    }

    private void DirtyFilter()
    {
        dirtyFilter = true;
    }

    private bool IsConfirmKey( KeyboardEventArgs eventArgs )
    {
        if ( ConfirmKey.IsNullOrEmpty() )
            return false;

        return ConfirmKey.Contains( eventArgs.Code ) && !eventArgs.IsModifierKey();
    }

    private bool IsSuggestedActiveItem( TItem item )
    {
        return ( IsSuggestSelectedItems && IsSelectedItem( item ) );
    }

    /// <summary>
    /// Gets whether the <typeparamref name="TValue"/> is selected.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsSelectedvalue( TValue value )
    {
        if ( IsMultiple )
            return SelectedValues?.Contains( value ) ?? false;
        else
            return SelectedValue?.IsEqual( value ) ?? false;
    }

    /// <summary>
    /// Gets whether the <typeparamref name="TItem"/> is selected.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool IsSelectedItem( TItem item )
    {
        if ( IsMultiple )
            return SelectedValues?.Contains( ValueField.Invoke( item ) ) ?? false;
        else
            return SelectedValue.IsEqual( ValueField.Invoke( item ) );
    }

    private object GetValidationValue()
    {
        if ( IsMultiple )
        {
            if ( SelectedValues is not null )
                return SelectedValues;

            return FreeTyping ? SelectedTexts : null;
        }

        return FreeTyping ? Search?.ToString() : SelectedValue?.ToString();
    }

    /// <summary>
    /// Gets the text of the <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string GetItemText( TValue value )
    {
        var item = GetItemByValue( value );

        return item is null
            ? string.Empty
            : GetItemText( item );
    }

    /// <summary>
    /// Gets the text of the <typeparamref name="TItem"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private string GetItemText( TItem item )
    {
        if ( item is null )
            return string.Empty;

        return TextField?.Invoke( item ) ?? string.Empty;
    }



    /// <summary>
    /// Gets multiple values from <see cref="Data"/> by using the provided <see cref="TextField"/> and <see cref="ValueField"/>.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerable<TValue> GetItemValues( string text )
    {
        var items = GetItemsByText( text );

        return items.IsNullOrEmpty()
            ? Array.Empty<TValue>()
            : items.Select( GetItemValue );
    }

    /// <summary>
    /// Gets a <typeparamref name="TValue"/> from <see cref="Data"/> by using the provided <see cref="TextField"/>.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private TValue GetItemValue( string text )
    {
        var item = GetItemByText( text );

        return item is null
            ? default
            : GetItemValue( item );
    }

    /// <summary>
    /// Gets a <typeparamref name="TValue"/> from <see cref="Data"/> by using the provided <see cref="ValueField"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private TValue GetItemValue( TItem item )
    {
        if ( item is null || ValueField == null )
            return default;

        return ValueField.Invoke( item );
    }

    /// <summary>
    /// Gets a <typeparamref name="TItem"/> from <see cref="Data"/> by using the provided <see cref="ValueField"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TItem GetItemByValue( TValue value )
        => Data is not null
               ? Data.FirstOrDefault( x => ValueField( x ).IsEqual( value ) )
               : default;

    /// <summary>
    /// Gets a <typeparamref name="TItem"/> from <see cref="Data"/> by using the provided <see cref="TextField"/>.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public TItem GetItemByText( string text )
        => Data is not null
               ? Data.FirstOrDefault( x => TextField( x ).IsEqual( text ) )
               : default;

    /// <summary>
    /// Gets multiple items from <see cref="Data"/> by using the provided <see cref="TextField"/>.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public IEnumerable<TItem> GetItemsByText( string text )
        => Data is not null
               ? Data.Where( x => TextField( x ).IsEqual( text ) )
               : Array.Empty<TItem>();

    /// <summary>
    /// Gets a <typeparamref name="TValue"/> from <see cref="SelectedValues"/> by using the provided <see cref="TextField"/> and <see cref="ValueField"/>.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private TValue GetValueByText( string text )
    {
        if ( SelectedValues is not null && SelectedTexts is not null )
        {
            var selectedValueIndex = SelectedTexts.IndexOf( text );

            if ( selectedValueIndex > -1 && selectedValueIndex < SelectedValues.Count )
                return SelectedValues[selectedValueIndex];
        }

        var item = GetItemByText( text );

        return item is null
            ? default
            : GetItemValue( item );
    }

    private Color GetMultipleBadgeColor() => Disabled
        ? MultipleDisabledBadgeColor
        : MultipleBadgeColor;

    private bool GetItemDisabled( TItem item )
    {
        return DisabledItem is not null && DisabledItem.Invoke( item );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Suggests already selected option(s) when presenting the options.
    /// </summary>
    private bool IsSuggestSelectedItems => SuggestSelectedItems || SelectionMode == AutocompleteSelectionMode.Checkbox;

    /// <summary>
    /// True if user is using <see cref="ReadData"/> for loading the data.
    /// </summary>
    public bool ManualReadMode => ReadData.HasDelegate;

    /// <summary>
    /// True if user is using <see cref="ReadData"/> and <see cref="Virtualize"/> for loading the data.
    /// </summary>
    public bool VirtualizeManualReadMode => ReadData.HasDelegate && Virtualize;

    /// <summary>
    /// Returns true if ReadData will be invoked.
    /// </summary>
    protected bool Loading { get; set; }

    /// <inheritdoc/>
    public override object ValidationValue
        => CustomValidationValue is not null
            ? CustomValidationValue.Invoke()
            : GetValidationValue();

    /// <summary>
    /// Gets the Element Id
    /// </summary>
    public string InputElementId { get; private set; }

    /// <summary>
    /// Gets the dropdown target element id used for positioning.
    /// </summary>
    protected string DropdownMenuTargetElementId
        => IsMultiple && !string.IsNullOrEmpty( ElementId )
            ? ElementId
            : InputElementId;

    /// <summary>
    /// Gets the dropdown CSS styles.
    /// </summary>
    protected string CssStyle
    {
        get
        {
            var sb = new StringBuilder();

            if ( MaxMenuHeight != null )
                sb.Append( $"--autocomplete-menu-max-height: {MaxMenuHeight};" );

            if ( Style != null )
                sb.Append( Style );

            return sb.ToString();
        }
    }

    /// <summary>
    /// Gets or sets the currently active item index.
    /// </summary>
    protected int ActiveItemIndex { get; set; } = -1;

    /// <summary>
    /// Gets or sets the search field focus state.
    /// </summary>
    protected bool TextFocused { get; set; }

    /// <summary>
    /// True if the dropdown menu should be visible.
    /// Takes into account whether menu was open and whether CloseOnSelection is set to false.
    /// </summary>
    protected bool DropdownVisible
        => canShowDropDown && IsTextSearchable && TextFocused && HasFilteredData;

    /// <summary>
    /// True if the not found content should be visible.
    /// </summary>
    protected bool NotFoundVisible
        => !FreeTyping && canShowDropDown && NotFoundContent is not null && IsTextSearchable && !Loading && !HasFilteredData;

    /// <summary>
    /// True if the free typing not found content should be visible.
    /// </summary>
    protected bool FreeTypingNotFoundVisible
        => FreeTyping && canShowDropDown && FreeTypingNotFoundTemplate is not null && IsTextSearchable && !Loading && !HasFilteredData;

    /// <summary>
    /// True if the text complies to the search requirements
    /// </summary>
    protected bool IsTextSearchable
        => Search?.Length >= MinLength;

    /// <summary>
    /// True if the filtered data exists
    /// </summary>
    protected bool HasFilteredData
        => FilteredData.Count > 0;

    /// <summary>
    /// Gets the custom class-names for dropdown element.
    /// </summary>
    protected string DropdownClassNames
    {
        get
        {
            var classBuilder = new StringBuilder();

            if ( !string.IsNullOrEmpty( Class ) )
            {
                classBuilder.Append( Class );
                classBuilder.Append( ' ' );
            }

            classBuilder.Append( "b-is-autocomplete" );

            if ( IsMultiple )
            {
                classBuilder.Append( " b-is-autocomplete-multipleselection" );
            }

            if ( TextFocused )
            {
                classBuilder.Append( " focus" );
            }

            string validationClass = ClassProvider.TextInputValidation( ParentValidation?.Status ?? ValidationStatus.None );

            if ( !string.IsNullOrEmpty( validationClass ) )
            {
                classBuilder.Append( ' ' );
                classBuilder.Append( validationClass );
            }

            return classBuilder.ToString();
        }
    }

    /// <summary>
    /// Gets the custom class-names for dropdown element.
    /// </summary>
    protected string DropdownItemClassNames( int index )
        => $"b-is-autocomplete-suggestion {ClassProvider.AutocompleteItemFocus( ActiveItemIndex == index )} {( SelectionMode == AutocompleteSelectionMode.Checkbox ? "b-is-autocomplete-suggestion-checkbox" : string.Empty )}";

    /// <summary>
    /// Provides an index based id for the dropdown suggestion items.
    /// </summary>
    protected string DropdownItemId( int index )
        => $"b-is-autocomplete-suggestion-{index}";

    /// <summary>
    /// Tracks whether the Autocomplete is in a multiple selection state.
    /// </summary>
    protected bool IsMultiple => SelectionMode == AutocompleteSelectionMode.Multiple || SelectionMode == AutocompleteSelectionMode.Checkbox;

    /// <summary>
    /// Gets or sets the <see cref="IJSClosableModule"/> instance.
    /// </summary>
    [Inject] public IJSClosableModule JSClosableModule { get; set; }

    /// <summary>
    /// Gets or sets the license checker for the user session.
    /// </summary>
    [Inject] internal BlazoriseLicenseChecker LicenseChecker { get; set; }

    /// <summary>
    /// Defines the method by which the search will be done.
    /// </summary>
    [Parameter] public AutocompleteFilter Filter { get; set; } = AutocompleteFilter.StartsWith;

    /// <summary>
    /// The minimum number of characters a user must type before a search is performed. Set this to 0 to make the Autocomplete function like a dropdown.
    /// </summary>
    [Parameter] public int MinLength { get; set; } = 1;

    /// <summary>
    /// Specifies the maximum number of characters allowed in the input element.
    /// </summary>
    [Parameter] public int? MaxEntryLength { get; set; }

    /// <summary>
    /// Sets the maximum height of the dropdown menu.
    /// </summary>
    [Parameter] public string MaxMenuHeight { get; set; }

    /// <summary>
    /// Sets the placeholder for the empty search.
    /// </summary>
    [Parameter] public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the autocomplete data-source.
    /// </summary>
    [EditorRequired]
    [Parameter]
    public IEnumerable<TItem> Data
    {
        get => data;
        set
        {
            if ( !data.IsEqual( value ) )
            {
                data = value;
                // make sure everything is recalculated
                DirtyFilter();
            }
        }
    }

    /// <summary>
    /// Event handler used to load data manually based on the current search value.
    /// </summary>
    [Parameter] public EventCallback<AutocompleteReadDataEventArgs> ReadData { get; set; }

    /// <summary>
    /// Event handler used to detect when the autocomplete is closed.
    /// </summary>
    [Parameter] public EventCallback<AutocompleteClosedEventArgs> Closed { get; set; }

    /// <summary>
    /// Event handler used to detect when the autocomplete is opened.
    /// </summary>
    [Parameter] public EventCallback Opened { get; set; }

    /// <summary>
    /// Gets the data after all of the filters have being applied.
    /// </summary>
    protected IList<TItem> FilteredData
    {
        get
        {
            if ( dirtyFilter )
                FilterData();

            return filteredData;
        }
    }

    /// <summary>
    /// Allows the value to not be on the data source.
    /// The value will be bound to the <see cref="SelectedText"/>
    /// </summary>
    [Parameter] public bool FreeTyping { get; set; }

    /// <summary>
    /// Method used to get the display field from the supplied data source.
    /// </summary>
    [EditorRequired]
    [Parameter]
    public Func<TItem, string> TextField { get; set; }

    /// <summary>
    /// Method used to get the value field from the supplied data source.
    /// </summary>
    [EditorRequired]
    [Parameter] public Func<TItem, TValue> ValueField { get; set; }

    /// <summary>
    /// Currently selected item value.
    /// </summary>
    [Parameter]
    public TValue SelectedValue
    {
        get => Value;
        set => Value = value;
    }

    /// <summary>
    /// Occurs after the selected value has changed.
    /// </summary>
    [Parameter]
    public EventCallback<TValue> SelectedValueChanged
    {
        get => ValueChanged;
        set => ValueChanged = value;
    }

    /// <summary>
    /// Gets or sets an expression that identifies the selected value.
    /// </summary>
    [Parameter]
    public Expression<Func<TValue>> SelectedValueExpression
    {
        get => ValueExpression;
        set => ValueExpression = value;
    }

    /// <summary>
    /// Gets or sets the currently selected item text.
    /// </summary>
    [Parameter]
    public string SelectedText { get; set; }

    /// <summary>
    /// Gets or sets the currently selected item text.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedTextChanged { get; set; }

    /// <summary>
    /// Gets or sets the currently selected item text.
    /// </summary>
    [Parameter]
    public string Search
    {
        get => currentSearch ?? currentSearchParam ?? string.Empty;
        set => currentSearchParam = value;
    }

    /// <summary>
    /// Occurs on every search text change.
    /// </summary>
    [Parameter] public EventCallback<string> SearchChanged { get; set; }

    /// <summary>
    /// If true, the searched text will be highlighted in the dropdown list items based on <see cref="Search"/> value.
    /// </summary>
    [Parameter] public bool HighlightSearch { get; set; }

    /// <summary>
    /// Defines the background color of the search field.
    /// </summary>
    [Parameter] public Background SearchBackground { get; set; }

    /// <summary>
    /// Defines the text color of the search field.
    /// </summary>
    [Parameter] public TextColor SearchTextColor { get; set; }

    /// <summary>
    /// Defines class for search field.
    /// </summary>
    [Parameter] public string SearchClass { get; set; }

    /// <summary>
    /// Defines style for search field.
    /// </summary>
    [Parameter] public string SearchStyle { get; set; }

    /// <summary>
    /// Gets the search field class-names with validation state.
    /// </summary>
    protected string SearchClassNames
    {
        get
        {
            return string.IsNullOrEmpty( SearchClass )
                ? null
                : SearchClass;
        }
    }

    /// <summary>
    /// Currently selected items values.
    /// Used when multiple selection is set.
    /// </summary>
    [Parameter]
    public List<TValue> SelectedValues
    {
        get => selectedValuesParam;
        set => selectedValuesParam = ( value == null ? null : new( value ) );
    }

    /// <summary>
    /// Occurs after the selected values have changed.
    /// Used when multiple selection is set.
    /// </summary>
    [Parameter] public EventCallback<List<TValue>> SelectedValuesChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the selected values.
    /// Used when multiple selection is set.
    /// </summary>
    [Parameter] public Expression<Func<List<TValue>>> SelectedValuesExpression { get; set; }

    /// <summary>
    /// Currently selected items texts.
    /// Used when multiple selection is set.
    /// </summary>
    [Parameter]
    public List<string> SelectedTexts
    {
        get => selectedTextsParam;
        set => selectedTextsParam = ( value == null ? null : new( value ) );
    }

    /// <summary>
    /// Occurs after the selected texts have changed.
    /// Used when multiple selection is set.
    /// </summary>
    [Parameter] public EventCallback<List<string>> SelectedTextsChanged { get; set; }

    /// <summary>
    /// If true the text in will be changed after each key press.
    /// </summary>
    /// <remarks>
    /// Note that setting this will override global settings in <see cref="BlazoriseOptions.Immediate"/>.
    /// </remarks>
    [Parameter] public bool? Immediate { get; set; }

    /// <summary>
    /// If true the entered text will be slightly delayed before submitting it to the internal value.
    /// </summary>
    [Parameter] public bool? Debounce { get; set; }

    /// <summary>
    /// Interval in milliseconds that entered text will be delayed from submitting to the internal value.
    /// </summary>
    [Parameter] public int? DebounceInterval { get; set; }

    /// <summary>
    /// Specifies the not found content to be rendered inside this <see cref="Autocomplete{TItem, TValue}"/> when no data is found.
    /// </summary>
    [Parameter] public RenderFragment<string> NotFoundContent { get; set; }

    /// <summary>
    /// Specifies the not found content to be rendered inside this <see cref="Autocomplete{TItem, TValue}"/> when no data is found and FreeTyping is enabled.
    /// </summary>
    [Parameter] public RenderFragment<string> FreeTypingNotFoundTemplate { get; set; }

    /// <summary>
    /// Occurs on every search text change where the data does not contain the text being searched.
    /// </summary>
    [Parameter] public EventCallback<string> NotFound { get; set; }

    /// <summary>
    /// Handler for custom filtering on Autocomplete's data source.
    /// </summary>
    [Parameter] public Func<TItem, string, bool> CustomFilter { get; set; }

    /// <summary>
    /// Sets the Badge color for the multiple selection values. Used when multiple selection is set.
    /// </summary>
    [Parameter] public Color MultipleBadgeColor { get; set; } = Color.Primary;

    /// <summary>
    /// Sets the disabled Badge color for the multiple selection values. Used when multiple selection is set.
    /// </summary>
    [Parameter] public Color MultipleDisabledBadgeColor { get; set; } = Color.Light;

    /// <summary>
    /// Specifies the item content to be rendered inside each dropdown item.
    /// </summary>
    [Parameter] public RenderFragment<ItemContext<TItem, TValue>> ItemContent { get; set; }

    /// <summary>
    /// Specifies whether <see cref="Autocomplete{TItem, TValue}"/> dropdown closes on selection. This is only evaluated when multiple selection is set.
    /// Defauls to true.
    /// </summary>
    [Parameter] public bool CloseOnSelection { get; set; } = true;

    /// <summary>
    /// Suggests already selected option(s) when presenting the options.
    /// </summary>
    [Parameter] public bool SuggestSelectedItems { get; set; }

    /// <summary>
    /// Gets or sets an array of the keyboard pressed values for the ConfirmKey.
    /// If this is null or empty, there will be no confirmation key.
    /// <para>Defauls to: { "Enter", "NumpadEnter", "Tab" }.</para>
    /// </summary>
    /// <remarks>
    /// If the value has a printed representation, this attribute's value is the same as the char attribute.
    /// Otherwise, it's one of the key value strings specified in 'Key values'.
    /// </remarks>
    [Parameter] public string[] ConfirmKey { get; set; } = new[] { "Enter", "NumpadEnter" };

    /// <summary>
    /// Gets or sets whether <see cref="Autocomplete{TItem, TValue}"/> auto preselects the first item displayed on the dropdown.
    /// Defauls to true.
    /// </summary>
    [Parameter] public bool AutoPreSelect { get; set; } = true;

    /// <summary>
    /// Gets or sets the <see cref="Autocomplete{TItem, TValue}"/> Selection Mode.
    /// </summary>
    [Parameter] public AutocompleteSelectionMode SelectionMode { get; set; } = AutocompleteSelectionMode.Default;

    /// <summary>
    /// Gets or sets the whether first item in the list should be selected
    /// </summary>
    [Parameter] public bool AutoSelectFirstItem { get; set; }

    /// <summary>
    /// Gets or sets whether the Autocomplete will use the Virtualize functionality.
    /// </summary>
    [Parameter] public bool Virtualize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items. Used only when <see cref="ReadData"/> and <see cref="Virtualize"/> is used to load the data.
    /// </summary>
    /// <remarks>
    /// This field must be set only when <see cref="ReadData"/> and <see cref="Virtualize"/> is used to load the data.
    /// </remarks>
    [Parameter] public int? TotalItems { get; set; }

    /// <summary>
    /// Specifies the content to be rendered for each tag (multiple selected item).
    /// </summary>
    [Parameter] public RenderFragment<AutocompleteTagContext<TItem, TValue>> TagTemplate { get; set; }

    /// <summary>
    /// Occurs after the search box text has changed.
    /// </summary>
    [Parameter] public EventCallback<string> SearchTextChanged { get; set; }

    /// <summary>
    /// Occurs when a key is pressed down while the search box has focus.
    /// </summary>
    [Parameter] public EventCallback<KeyboardEventArgs> SearchKeyDown { get; set; }

    /// <summary>
    /// Occurs when the search box gains or loses focus.
    /// </summary>
    [Parameter] public EventCallback<FocusEventArgs> SearchFocus { get; set; }

    /// <summary>
    /// The blur event fires when the search box has lost focus.
    /// </summary>
    [Parameter] public EventCallback<FocusEventArgs> SearchBlur { get; set; }

    /// <summary>
    /// Defines the positioning strategy of the dropdown menu as a 'floating' element.
    /// </summary>
    [Parameter] public DropdownPositionStrategy PositionStrategy { get; set; } = DropdownPositionStrategy.Absolute;

    /// <summary>
    /// Method used to get the determine if the item should be disabled.
    /// </summary>
    [Parameter] public Func<TItem, bool> DisabledItem { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether parent dropdown menus should be closed when this component is activated.
    /// </summary>
    /// <remarks>Set this property to <see langword="true"/> to automatically close any open parent dropdowns
    /// when the component is triggered. This can be useful for ensuring only one dropdown is open at a time in nested
    /// menu scenarios.</remarks>
    [Parameter] public bool CloseParentDropdowns { get; set; }

    #endregion
}