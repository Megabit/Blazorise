#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseSelect<TValue> : BaseInputComponent<IReadOnlyList<TValue>>
    {
        #region Members

        private List<BaseSelectItem<TValue>> selectItems;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Select() )
                .If( () => ClassProvider.SelectSize( Size ), () => Size != Size.None )
                .If( () => ClassProvider.SelectValidation( ParentValidation?.Status ?? ValidationStatus.None ), () => ParentValidation?.Status != ValidationStatus.None );

            base.RegisterClasses();
        }

        protected async void SelectionChangedHandler( UIChangeEventArgs e )
        {
            Value = await JSRunner.GetSelectedOptions<TValue>( ElementId );
            Console.WriteLine( "V: " + string.Join( ";", Value ) );
            SelectedValueChanged?.Invoke( Value.FirstOrDefault() );
            SelectedValuesChanged?.Invoke( Value );
        }

        internal void Register( BaseSelectItem<TValue> selectItem )
        {
            if ( selectItem == null )
                return;

            if ( selectItems == null )
                selectItems = new List<BaseSelectItem<TValue>>();

            if ( !selectItems.Contains( selectItem ) )
            {
                selectItems.Add( selectItem );

                ClassMapper.Dirty();

                //if ( selectItems?.Count > 1 ) // must find a better way to refresh
                //    StateHasChanged();
            }
        }

        internal bool IsSelected( BaseSelectItem<TValue> selectItem )
        {
            foreach ( var val in Value )
            {
                if ( EqualityComparer<TValue>.Default.Equals( selectItem.Value, default ) )
                {
                    return true;
                }
            }

            return false;
            //return Value?.Contains( selectItem.Value ) == true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Specifies that multiple options can be selected at once.
        /// </summary>
        [Parameter] protected bool IsMultiple { get; set; }

        /// <summary>
        /// Gets or sets the selected item value.
        /// </summary>
        [Parameter]
        protected TValue SelectedValue
        {
            get
            {
                return Value.FirstOrDefault();
            }
            set
            {
                Value = new TValue[] { value };

                StateHasChanged();
            }
        }

        [Parameter]
        protected IReadOnlyList<TValue> SelectedValues
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;

                StateHasChanged();
            }
        }

        /// <summary>
        /// Occurs when the selected item value has changed.
        /// </summary>
        [Parameter] protected Action<TValue> SelectedValueChanged { get; set; }

        [Parameter] protected Action<IReadOnlyList<TValue>> SelectedValuesChanged { get; set; }

        #endregion
    }
}
