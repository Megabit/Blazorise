#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseSelect<TValue> : BaseInputComponent<TValue>
    {
        #region Members

        private IReadOnlyList<TValue> internalMultiValue;

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
            if ( IsMultiple )
            {
                // when multiple selection is enabled we need to use javascript to get the list of selected items
                internalMultiValue = await JSRunner.GetSelectedOptions<TValue>( ElementId );

                // changed event must be called before validation
                SelectedValuesChanged?.Invoke( internalMultiValue );

                ParentValidation?.UpdateInputValue( internalMultiValue );
            }
            else
            {
                if ( Converters.TryChangeType<TValue>( e.Value, out var value ) )
                    internalValue = value;
                else
                    internalValue = default;

                // changed event must be called before validation
                SelectedValueChanged?.Invoke( internalValue );

                ParentValidation?.UpdateInputValue( internalValue );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Specifies that multiple items can be selected.
        /// </summary>
        [Parameter] protected internal bool IsMultiple { get; set; }

        /// <summary>
        /// Gets or sets the selected item value.
        /// </summary>
        [Parameter]
        protected internal TValue SelectedValue
        {
            get { return internalValue; }
            set
            {
                if ( !EqualityComparer<TValue>.Default.Equals( this.internalValue, value ) )
                {
                    internalValue = value;

                    ParentValidation?.UpdateInputValue( internalValue );
                }
            }
        }

        /// <summary>
        /// Gets or sets the multiple selected item values.
        /// </summary>
        [Parameter]
        protected internal IReadOnlyList<TValue> SelectedValues
        {
            get { return internalMultiValue; }
            set
            {
                internalMultiValue = value;

                ParentValidation?.UpdateInputValue( internalMultiValue );
            }
        }

        /// <summary>
        /// Occurs when the selected item value has changed.
        /// </summary>
        [Parameter] protected Action<TValue> SelectedValueChanged { get; set; }

        /// <summary>
        /// Occurs when the selected items value has changed (only when <see cref="IsMultiple"/>==true).
        /// </summary>
        [Parameter] protected Action<IReadOnlyList<TValue>> SelectedValuesChanged { get; set; }

        #endregion
    }
}
