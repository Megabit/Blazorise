#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
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
public partial class Autocomplete<TItem, TValue> : BaseAfterRenderComponent, IAsyncDisposable
{
    private class NullableT<T>
    {
        public NullableT( T t ) => Value = t;

        public T Value;

        public static implicit operator T( NullableT<T> nullable ) => nullable == null ? default : nullable.Value;
    }

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
    /// Reference to the TextEdit component.
    /// </summary>
    private TextEdit textEditRef;

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

    private NullableT<TValue> selectedValue;
    private TValue selectedValueParam;

    private List<TValue> selectedValuesParam;
    private List<string> selectedTextsParam;

    private Validation validationRef;

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
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<string>( nameof( Search ), out var paramCurrentSearch ) && currentSearchParam != paramCurrentSearch )
        {
            currentSearch = null;
        }

        bool selectedValueParamChanged = false;

        if ( parameters.TryGetValue<TValue>( nameof( SelectedValue ), out var paramSelectedValue ) && !selectedValueParam.IsEqual( paramSelectedValue ) )
        {
            selectedValue = null;
            selectedValueParamChanged = true;
        }

        var selectedTextParamChanged = parameters.TryGetValue<string>( nameof( SelectedText ), out var paramSelectedText ) && SelectedText != paramSelectedText;

        var selectedValuesParamChanged = parameters.TryGetValue<IEnumerable<TValue>>( nameof( SelectedValues ), out var paramSelectedValues )
            && !selectedValuesParam.AreEqualOrdered( paramSelectedValues );

        var selectedTextsParamChanged = parameters.TryGetValue<IEnumerable<string>>( nameof( SelectedTexts ), out var paramSelectedTexts )
            && !selectedTextsParam.AreEqualOrdered( paramSelectedTexts );

        await base.SetParametersAsync( parameters );

        await SynchronizeSingle( selectedValueParamChanged, selectedTextParamChanged );
        await SynchronizeMultiple( selectedValuesParamChanged, selectedTextsParamChanged );
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
            return new( Data.ToList(), TotalItems.HasValue ? TotalItems.Value : default );
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
                    selectedValue = new( default );
                    await SelectedValueChanged.InvokeAsync( selectedValue );
                }
                else
                {
                    NullableT<TValue> value = new( GetItemValue( item ) );
                    if ( !SelectedValue.IsEqual( value ) )
                    {
                        selectedValue = new( value );
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

    private async Task SynchronizeMultiple( bool selectedValuesParamChanged, bool selectedTextsParamChanged )
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

        if ( selectedValuesParamChanged && selectedValuesParam is not null && !Data.IsNullOrEmpty() )
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

        ExecuteAfterRender( async () => await JSClosableModule.RegisterLight( ElementRef ) );

        if ( ManualReadMode )
            await Reload();

        if ( AutoSelectFirstItem && !IsMultiple )
        {
            if ( HasFilteredData )
            {
                currentSearch = SelectedText = GetItemText( FilteredData.First() );
                selectedValue = new( GetItemValue( FilteredData.First() ) );

                await Task.WhenAll(
                    InvokeSearchChanged( currentSearch ),
                    SelectedTextChanged.InvokeAsync( SelectedText ),
                    SelectedValueChanged.InvokeAsync( selectedValue )
                );
            }
        }

        await base.OnInitializedAsync();
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
        await SearchFocus.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handles the search field onblur event.
    /// </summary>
    /// <param name="eventArgs">Event arguments.</param>
    /// <returns>Returns awaitable task</returns>
    protected async Task OnTextBlurHandler( FocusEventArgs eventArgs )
    {
        if ( SelectionMode == AutocompleteSelectionMode.Checkbox )
        {
            //Workaround for the issue where the dropdown closes when clicking on the checkbox
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
                await textEditRef.Focus();
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

            await SearchBlur.InvokeAsync( eventArgs );
        }
    }

    private async Task InvokeSearchChanged( string searchValue )
    {
#pragma warning disable CS0618 // Type or member is obsolete
        await Task.WhenAll(
        CurrentSearchChanged.InvokeAsync( searchValue ),
        SearchChanged.InvokeAsync( searchValue ) );
#pragma warning restore CS0618 // Type or member is obsolete
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
            if ( !IsSelectedvalue( GetItemValue( Search ) ) )
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
        clickFromCheck = ( SelectionMode == AutocompleteSelectionMode.Checkbox );

        //TODO : Once Multiple is deprecated we may remove the && !IsMultiple condition
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
            selectedValue = new( selectedTValue );
            currentSearch = SelectedText = GetItemText( selectedValue );
            DirtyFilter();

            await Task.WhenAll(
                SelectedValueChanged.InvokeAsync( selectedValue ),
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
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );

            Loading = true;

            if ( !cancellationTokenSource.Token.IsCancellationRequested && IsTextSearchable )
            {
                await ReadData.InvokeAsync( new( Search, cancellationToken: cancellationTokenSource.Token ) );
                await Task.Yield(); // rebind Data after ReadData
            }
        }
        finally
        {
            Loading = false;
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
            return;

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

    private Task Revalidate()
    {
        if ( textEditRef is not null )
        {
            return textEditRef.Revalidate();
        }
        return Task.CompletedTask;
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

        selectedValue = new( default );
        if ( notifyChange )
            await SelectedValueChanged.InvokeAsync( default );
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

        SelectedTexts.Remove( text );
        await SelectedTextsChanged.InvokeAsync( SelectedTexts );

        if ( SelectionMode == AutocompleteSelectionMode.Multiple )
            DirtyFilter();
    }

    private async Task RemoveMultipleValue( TValue value )
    {
        if ( SelectedValues is null )
            return;

        SelectedValues.Remove( value );
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
        if ( Disabled )
            return;

        await RemoveMultipleText( text );
        await RemoveMultipleValue( GetValueByText( text ) );

        if ((validationRef.ParentValidations?.Mode ?? ValidationMode.Auto) == ValidationMode.Auto)
            await validationRef.ValidateAsync();

        DirtyFilter();
    }

    /// <summary>
    /// Removes a Multiple Selection.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task RemoveMultipleTextAndValue( TValue value )
    {
        await RemoveMultipleText( GetItemText( value ) );
        await RemoveMultipleValue( value );

        if ((validationRef.ParentValidations?.Mode ?? ValidationMode.Auto) == ValidationMode.Auto)
            await validationRef.ValidateAsync();

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
                        where text.IndexOf( Search, 0, StringComparison.CurrentCultureIgnoreCase ) >= 0
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

        var maxRowsLimit = LicenseChecker.GetAutoCompleteRowsLimit();

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
    public Task Focus( bool scrollToElement = true )
    {
        if ( textEditRef != null )
            return textEditRef.Focus( scrollToElement );

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
    /// Determines if Autocomplete can be closed
    /// </summary>
    /// <returns>True if Autocomplete can be closed.</returns>
    ///
    [Obsolete( "IsSafeToClose is deprecated. This API now always returns true." )]
    public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChild )
    {
        return Task.FromResult( true );
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

    private string GetValidationValue()
    {
        if ( IsMultiple )
        {
            return SelectedTexts.IsNullOrEmpty() ? string.Empty : string.Join( ';', SelectedTexts );
        }

        return FreeTyping ? Search?.ToString() : SelectedValue?.ToString();
    }

    private string GetItemText( TValue value )
    {
        var item = GetItemByValue( value );

        return item is null
            ? string.Empty
            : GetItemText( item );
    }

    private string GetItemText( TItem item )
    {
        if ( item is null )
            return string.Empty;

        return TextField?.Invoke( item ) ?? string.Empty;
    }

    private TValue GetItemValue( string text )
    {
        var item = GetItemByText( text );

        return item is null
            ? default
            : GetItemValue( item );
    }

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
    /// Gets a <typeparamref name="TValue"/> from <see cref="SelectedValues"/> by using the provided <see cref="TextField"/> and <see cref="ValueField"/>.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private TValue GetValueByText( string text )
        => SelectedValues is not null
        ? SelectedValues.FirstOrDefault( x => GetItemText( x ) == text )
        : default;

    private Color GetMultipleBadgeColor() => Disabled
        ? MultipleDisabledBadgeColor
        : MultipleBadgeColor;

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

    /// <summary>
    /// Gets the Element Reference
    /// </summary>
    public ElementReference ElementRef => textEditRef.ElementRef;

    /// <summary>
    /// Gets the Element Id
    /// </summary>
    public string InputElementId { get; private set; }

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
        => $"{Class} b-is-autocomplete {( IsMultiple ? "b-is-autocomplete-multipleselection" : string.Empty )} {( TextFocused ? "focus" : string.Empty )}";

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
#pragma warning disable CS0618 // Type or member is obsolete
    protected bool IsMultiple => Multiple || SelectionMode == AutocompleteSelectionMode.Multiple || SelectionMode == AutocompleteSelectionMode.Checkbox;
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    /// Gets or sets the classname provider.
    /// </summary>
    [Inject] protected IClassProvider ClassProvider { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IJSClosableModule"/> instance.
    /// </summary>
    [Inject] public IJSClosableModule JSClosableModule { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Gets or set the IdGenerator.
    /// </summary>
    [Inject] public IIdGenerator IdGenerator { get; set; }

    /// <summary>
    /// Gets or sets the license checker for the user session.
    /// </summary>
    [Inject] internal BlazoriseLicenseChecker LicenseChecker { get; set; }

    /// <summary>
    /// Gets or sets the dropdown element id.
    /// </summary>
    [Parameter] public string ElementId { get; set; }

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
    /// Size of a search field.
    /// </summary>
    [Parameter] public Size? Size { get; set; }

    /// <summary>
    /// Prevents a user from entering a value to the search field.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

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
        get => selectedValue ?? selectedValueParam;
        set => selectedValueParam = value;
    }

    /// <summary>
    /// Occurs after the selected value has changed.
    /// </summary>
    [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

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
    [Obsolete( "CurrentSearch is deprecated and will be removed in a future version, please use Search instead." )]
    public string CurrentSearch
    {
        get => Search;
        set => Search = value;
    }

    /// <summary>
    /// Occurs on every search text change.
    /// </summary>
    [Obsolete( "CurrentSearchChanged is deprecated and will be removed in a future version, please use SearchChanged instead." )]
    [Parameter] public EventCallback<string> CurrentSearchChanged { get; set; }

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
    /// Custom class-name for dropdown element.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Custom styles for dropdown element.
    /// </summary>
    [Parameter] public string Style { get; set; }

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
    /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
    /// </summary>
    [Parameter] public int? TabIndex { get; set; }

    /// <summary>
    /// Validation handler used to validate selected value.
    /// </summary>
    [Parameter] public Action<ValidatorEventArgs> Validator { get; set; }

    /// <summary>
    /// Asynchronously validates the selected value.
    /// </summary>
    [Parameter] public Func<ValidatorEventArgs, CancellationToken, Task> AsyncValidator { get; set; }

    /// <summary>
    /// Captures all the custom attribute that are not part of Blazorise component.
    /// </summary>
    [Parameter( CaptureUnmatchedValues = true )]
    public Dictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Autocomplete{TItem, TValue}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

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
    /// Allows for multiple selection.
    /// </summary>
    [Obsolete( "Multiple parameter will be removed in a future version, please replace with SelectionMode.Multiple Parameter instead." )]
    [Parameter] public bool Multiple { get; set; }

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

    #endregion
}