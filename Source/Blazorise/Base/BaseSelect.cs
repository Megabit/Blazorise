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
            if ( IsMultiple )
            {
                // when multiple selection is enabled we need to use javascript to get the list of selected items
                multiValue = await JSRunner.GetSelectedOptions<TValue>( ElementId );

                // changed event must be called before validation
                SelectedValuesChanged?.Invoke( multiValue );

                ParentValidation?.UpdateInputValue( multiValue );
            }
            else
            {
                if ( Convertes.TryChangeType<TValue>( e.Value, out var value ) )
                    singleValue = value;
                else
                    singleValue = default;

                // changed event must be called before validation
                SelectedValueChanged?.Invoke( singleValue );

                ParentValidation?.UpdateInputValue( singleValue );
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
        protected internal IReadOnlyList<TValue> SelectedValues
        {
            get { return multiValue; }
            set
            {
                multiValue = value;

                ParentValidation?.UpdateInputValue( multiValue );
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
