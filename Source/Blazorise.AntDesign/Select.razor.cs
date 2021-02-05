#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.AntDesign
{
    public partial class Select<TValue> : Blazorise.Select<TValue>, ICloseActivator
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


        /// <summary>
        /// Internal string separator for selected values when Multiple mode is used.
        /// </summary>
        private const string MultipleValuesSeparator = ";"; // Let's hope ";" will be enough to distinguish the values!

        #endregion

        #region Methods

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing && Rendered )
            {
                // TODO: switch to IAsyncDisposable
                _ = JSRunner.UnregisterClosableComponent( this );

                DisposeDotNetObjectRef( dotNetObjectRef );
            }

            base.Dispose( disposing );
        }

        protected async Task OnSelectorClickHandler()
        {
            if ( Expanded )
                return;

            await Expand();
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
            // because sometimes input can have diferent offset based on the changes on the page. For example 
            // when validation is trigered the input can be pushed down by the error messages.
            elementInfo = await JSRunner.GetElementInfo( ElementRef, ElementId );

            await JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementRef );

            Expanded = true;
        }

        private async Task Collapse()
        {
            await JSRunner.UnregisterClosableComponent( this );

            Expanded = false;
        }

        private void ClearSelectedItems()
        {
            SelectedValue = default;
            SelectedValues = default;
        }

        protected Task OnMultipleValueClickHandler( TValue selectValue )
        {
            var list = new List<TValue>( SelectedValues ?? Enumerable.Empty<TValue>() );

            if ( list.Contains( selectValue ) )
                list.Remove( selectValue );

            return CurrentValueHandler( string.Join( MultipleValuesSeparator, list ) );
        }

        internal async Task NotifySelectValueChanged( TValue selectValue )
        {
            // We cuold just set SelectedValue(s) directly but that would skip validation process 
            // and we would also need to handle event handlers.
            // Thats why we need to call CurrentValueHandler that will trigger all that is required.
            if ( Multiple )
            {
                var list = new List<TValue>( SelectedValues ?? Enumerable.Empty<TValue>() );

                if ( list.Contains( selectValue ) )
                    list.Remove( selectValue );
                else
                    list.Add( selectValue );

                await CurrentValueHandler( string.Join( MultipleValuesSeparator, list ) );
            }
            else
            {
                await CurrentValueHandler( selectValue?.ToString() );

                await Collapse();
            }

            await InvokeAsync( StateHasChanged );
        }

        protected override Task<ParseValue<IReadOnlyList<TValue>>> ParseValueFromStringAsync( string value )
        {
            if ( string.IsNullOrEmpty( value ) )
                return Task.FromResult( ParseValue<IReadOnlyList<TValue>>.Empty );

            if ( Multiple )
            {
                // AntDesign does not use regular select element so there is no reason to call javascript like for
                // other css frameworks. That's why we only need to parse the string that we have used previously.
                return Task.FromResult( new ParseValue<IReadOnlyList<TValue>>( true, ParseMultipleValues( value ).ToList(), null ) );
            }
            else
            {
                if ( Converters.TryChangeType<TValue>( value, out var result ) )
                {
                    return Task.FromResult( new ParseValue<IReadOnlyList<TValue>>( true, new TValue[] { result }, null ) );
                }
                else
                {
                    return Task.FromResult( ParseValue<IReadOnlyList<TValue>>.Empty );
                }
            }
        }

        static IEnumerable<TValue> ParseMultipleValues( string csv )
        {
            if ( string.IsNullOrEmpty( csv ) )
                yield break;

            foreach ( var value in csv.Split( MultipleValuesSeparator ) )
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

        /// <summary>
        /// Gets the selected items render fragments.
        /// </summary>
        protected IEnumerable<RenderFragment> SelectedItems
        {
            get
            {
                foreach ( var selectedValue in SelectedValues )
                {
                    var item = SelectItems.FirstOrDefault( i => Convert.ToString( i.Value ) == Convert.ToString( selectedValue ) );

                    if ( item != null )
                    {
                        yield return item.ChildContent;
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
                if ( SelectedValue != null )
                {
                    var item = SelectItems.FirstOrDefault( i => Convert.ToString( i.Value ) == Convert.ToString( SelectedValue ) );

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

                if ( Multiple )
                    sb.Append( " ant-select-multiple" );
                else
                    sb.Append( " ant-select-single" );

                if ( Expanded )
                    sb.Append( " ant-select-open" );

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
                    sb.Append( " slide-up-leave slide-up-leave-active slide-up" );

                return sb.ToString();
            }
        }

        string DropdownStyleNames =>
                $"width: {(int)elementInfo.BoundingClientRect.Width}px; left: {(int)elementInfo.OffsetLeft}px; top: {(int)( elementInfo.OffsetTop + elementInfo.BoundingClientRect.Height )}px;";

        string DropdownInnerStyleNames
            => $"max-height: {( MaxVisibleItems == null ? 256 : MaxVisibleItems * 32 )}px; overflow-y: auto; overflow-anchor: none;";

        #endregion
    }
}
