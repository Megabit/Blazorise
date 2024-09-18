#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.AntDesign.Components;

public interface IAntSelect
{
    Task NotifySelectValueChanged( object selectValue );
}

public partial class Select<TValue> : Blazorise.Select<TValue>, ICloseActivator, IAsyncDisposable, IAntSelect
{
    #region Members

    private string selectorElementId;

    private string inputElementId;

    /// <summary>
    /// Holds the information about the element location and size.
    /// </summary>
    private DomElement elementInfo;

    /// <summary>
    /// Reference for the close activator used to control the closing of an component
    /// when the user leaves the component bounds.
    /// </summary>
    private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

    #endregion

    #region Methods

    protected override async Task OnFirstAfterRenderAsync()
    {
        await base.OnFirstAfterRenderAsync();

        dotNetObjectRef ??= CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

        // since we do some custom rendering we need to refresh component state to trigger first valid render.
        await InvokeAsync( StateHasChanged );
    }

    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            // TODO: switch to IAsyncDisposable
            await JSClosableModule.Unregister( this );

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;
        }

        await base.DisposeAsync( disposing );
    }

    protected Task OnSelectorClickHandler()
    {
        if ( Expanded )
            return Task.CompletedTask;

        return Expand();
    }

    public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChildClicked )
    {
        return Task.FromResult( Multiple
            ? closeReason == CloseReason.EscapeClosing || elementId == ElementId || elementId == SelectorElementId || elementId == InputElementId
            : true );
    }

    public async Task Close( CloseReason closeReason )
    {
        await Collapse();

        await InvokeAsync( StateHasChanged );
    }

    private async Task Expand()
    {
        // An element location must be known every time we need to show the dropdown. The reason is mainly
        // because sometimes input can have different offset based on the changes on the page. For example
        // when validation is triggered the input can be pushed down by the error messages.
        elementInfo = await JSUtilitiesModule.GetElementInfo( ElementRef, ElementId );

        await JSClosableModule.Register( dotNetObjectRef, ElementRef );

        Expanded = true;

        // automatically set focus to the dropdown so that we can make it auto-close on blur event
        ExecuteAfterRender( async () => await DropdownElementRef.FocusAsync() );
    }

    private async Task Collapse()
    {
        await JSClosableModule.Unregister( this );

        Expanded = false;
    }

    private void ClearSelectedItems()
    {
        Value = default;
    }

    public async Task NotifySelectValueChanged( object selectValue )
    {
        // We could just set SelectedValue(s) directly but that would skip validation process
        // and we would also need to handle event handlers.
        // Thats why we need to call CurrentValueHandler that will trigger all that is required.
        if ( Multiple && Value is IEnumerable<TValue> values )
        {
            var list = new List<object>( values?.Select( x => (object)x ) ?? Enumerable.Empty<object>() );

            if ( list.Any( x => x.IsEqual( selectValue ) ) )
                list.Remove( selectValue );
            else
                list.Add( selectValue );

            await CurrentValueHandler( string.Join( MULTIPLE_DELIMITER, list ) );
        }
        else
        {
            await CurrentValueHandler( selectValue?.ToString() );

            await Collapse();
        }

        await InvokeAsync( StateHasChanged );
    }

    //protected override Task<ParseValue<IReadOnlyList<TValue>>> ParseValueFromStringAsync( string value )
    //{
    //    if ( string.IsNullOrEmpty( value ) )
    //        return Task.FromResult( ParseValue<IReadOnlyList<TValue>>.Empty );

    //    if ( Multiple )
    //    {
    //        // AntDesign does not use regular select element so there is no reason to call javascript like for
    //        // other css frameworks. That's why we only need to parse the string that we have used previously.
    //        return Task.FromResult( new ParseValue<IReadOnlyList<TValue>>( true, ParseMultipleValues( value ).ToList(), null ) );
    //    }
    //    else
    //    {
    //        if ( Converters.TryChangeType<TValue>( value, out var result ) )
    //        {
    //            return Task.FromResult( new ParseValue<IReadOnlyList<TValue>>( true, new TValue[] { result }, null ) );
    //        }
    //        else
    //        {
    //            return Task.FromResult( ParseValue<IReadOnlyList<TValue>>.Empty );
    //        }
    //    }
    //}

    static IEnumerable<TValue> ParseMultipleValues( string csv )
    {
        if ( string.IsNullOrEmpty( csv ) )
            yield break;

        foreach ( var value in csv.Split( MULTIPLE_DELIMITER ) )
        {
            if ( Converters.TryChangeType<TValue>( value, out var result ) )
            {
                yield return result;
            }
        }
    }

    protected Task RemoveSelectedItem( TValue value )
    {
        return NotifySelectValueChanged( value );
    }

    protected Task OnSelectClearClickHandler()
    {
        ClearSelectedItems();

        return Task.CompletedTask;
    }

    // NOTE: Don't remove tabindex from dropdown `<div tabindex="-1" @onblur="@OnDropdownBlur">`! It must be defined for focus-out to work.
    protected async Task OnDropdownBlur( FocusEventArgs eventArgs )
    {
        if ( Expanded )
        {
            // Give enough time for other events to do their stuff before closing
            // the select menu.
            await Task.Delay( 250 );

            await Collapse();
        }
    }

    #endregion

    #region Properties

    protected bool Expanded { get; set; }

    protected string SelectorElementId
    {
        get => selectorElementId ??= IdGenerator.Generate;
        set => selectorElementId = value;
    }

    protected string InputElementId
    {
        get => inputElementId ??= IdGenerator.Generate;
        set => inputElementId = value;
    }

    protected ElementReference DropdownElementRef { get; set; }

    /// <summary>
    /// Gets the selected items render fragments.
    /// </summary>
    protected IEnumerable<RenderFragment> SelectedItems
    {
        get
        {
            if ( Value is IEnumerable<TValue> values )
            {
                foreach ( var selectedValue in values )
                {
                    var item = SelectItems.FirstOrDefault( i => i.CompareTo( selectedValue ) );

                    if ( item != null )
                    {
                        yield return item.ChildContent;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the render fragment for the selected option.
    /// </summary>
    protected RenderFragment SelectedItem
    {
        get
        {
            if ( Value is not null )
            {
                var item = SelectItems.FirstOrDefault( i => i.CompareTo( Value ) );

                return item?.ChildContent;
            }

            return null;
        }
    }

    string SelectListId =>
        $"select_list_{ElementId}";

    string ContainerClassNames
    {
        get
        {
            var sb = new StringBuilder( "ant-select ant-select-show-arrow" );

            if ( ThemeSize != Blazorise.Size.Default )
                sb.Append( $" ant-select-{ClassProvider.ToSize( ThemeSize )}" );

            if ( Multiple )
                sb.Append( " ant-select-multiple" );
            else
                sb.Append( " ant-select-single" );

            if ( Expanded )
                sb.Append( " ant-select-focused ant-select-open" );

            if ( Disabled )
                sb.Append( " ant-select-disabled" );

            return sb.ToString();
        }
    }

    string DropdownClassNames
    {
        get
        {
            var sb = new StringBuilder( "ant-select-dropdown ant-select-dropdown-placement-bottomLeft" );

            if ( Expanded )
                sb.Append( " slide-up-enter slide-up-enter-active slide-up" );
            else
                sb.Append( " ant-select-dropdown-hidden slide-up-leave slide-up-leave-active slide-up" );

            return sb.ToString();
        }
    }

    string DropdownStyleNames =>
        $"width: {(int)elementInfo.BoundingClientRect.Width}px; left: {(int)elementInfo.OffsetLeft}px; top: {(int)( elementInfo.OffsetTop + elementInfo.BoundingClientRect.Height )}px;";

    string DropdownInnerStyleNames
        => $"max-height: {( MaxVisibleItems == null ? 256 : MaxVisibleItems * 32 )}px; overflow-y: auto; overflow-anchor: none;";

    string InputAriaExpanded => Expanded ? "true" : "false";

    [Inject] public IJSClosableModule JSClosableModule { get; set; }

    #endregion
}