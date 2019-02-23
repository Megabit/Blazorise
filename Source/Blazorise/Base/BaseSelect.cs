#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseSelect<TValue> : BaseInputComponent<IReadOnlyList<TValue>>
    {
        #region Members

        private TValue singleValue;

        private IReadOnlyList<TValue> multiValue;

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

        protected async void HandleOnChanged( UIChangeEventArgs e )
        {
            // save result directly to the value in the base class
            var values = await JSRunner.GetSelectedOptions<TValue>( ElementId );

            if ( IsMultiple )
            {
                multiValue = values;
                SelectedValuesChanged?.Invoke( multiValue );

                ParentValidation?.UpdateInputValue( multiValue );
            }
            else
            {
                singleValue = values == null ? default : values.FirstOrDefault();
                SelectedValueChanged?.Invoke( singleValue );

                ParentValidation?.UpdateInputValue( singleValue );
            }
        }

        internal bool IsSelected( TValue itemValue )
        {
            if ( IsMultiple )
                return SelectedValues?.Contains( itemValue ) == true;

            return EqualityComparer<TValue>.Default.Equals( SelectedValue, itemValue );
        }

        //protected async void HandleOnChanged( UIChangeEventArgs e )
        //{
        //    // save result directly to the value in the base class
        //    Value = await JSRunner.GetSelectedOptions<TValue>( ElementId );

        //    SelectedValueChanged?.Invoke( SelectedValue );
        //    SelectedValuesChanged?.Invoke( SelectedValues );
        //}

        //internal bool IsSelected( TValue itemValue )
        //{
        //    return Value?.Contains( itemValue ) == true;
        //}

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
            get { return singleValue; }
            set
            {
                singleValue = value;

                ParentValidation?.UpdateInputValue( singleValue );
            }
        }

        /// <summary>
        /// Gets or sets the multiple selected item values.
        /// </summary>
        [Parameter]
        protected IReadOnlyList<TValue> SelectedValues
        {
            get { return multiValue; }
            set
            {
                multiValue = value;

                ParentValidation?.UpdateInputValue( multiValue );
            }
        }

        ///// <summary>
        ///// Gets or sets the selected item value.
        ///// </summary>
        //[Parameter]
        //protected TValue SelectedValue
        //{
        //    get
        //    {
        //        // make sure there is always one item available
        //        if ( Value?.Count == 0 )
        //            Value = new TValue[] { default };

        //        return Value[0];
        //    }
        //    set
        //    {
        //        Value = new TValue[] { value };

        //        //StateHasChanged();
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the multiple selected item values.
        ///// </summary>
        //[Parameter]
        //protected IReadOnlyList<TValue> SelectedValues
        //{
        //    get
        //    {
        //        return Value;
        //    }
        //    set
        //    {
        //        Value = value?.ToArray();

        //        //StateHasChanged();
        //    }
        //}

        /// <summary>
        /// Occurs when the selected item value has changed.
        /// </summary>
        [Parameter] protected Action<TValue> SelectedValueChanged { get; set; }

        [Parameter] protected Action<IReadOnlyList<TValue>> SelectedValuesChanged { get; set; }

        #endregion
    }
}
