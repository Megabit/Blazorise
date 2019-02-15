#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseSelect : BaseInputComponent<string[]>
    {
        #region Members

        private List<BaseSelectItem> selectItems;

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
            Value = await JSRunner.GetSelectedOptions( ElementId );
            SelectedValueChanged?.Invoke( string.Join( ";", Value ) );
        }

        internal void Register( BaseSelectItem selectItem )
        {
            if ( selectItem == null )
                return;

            if ( selectItems == null )
                selectItems = new List<BaseSelectItem>();

            if ( !selectItems.Contains( selectItem ) )
            {
                selectItems.Add( selectItem );

                ClassMapper.Dirty();

                //if ( selectItems?.Count > 1 ) // must find a better way to refresh
                //    StateHasChanged();
            }
        }

        internal bool IsSelected( BaseSelectItem selectItem )
        {
            return Value?.Contains( selectItem?.Value ) == true;
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
        protected string SelectedValue
        {
            get
            {
                return string.Join( ";", Value );
            }
            set
            {
                Value = value?.Split( ';' );

                StateHasChanged();
            }
        }

        /// <summary>
        /// Occurs when the selected item value has changed.
        /// </summary>
        [Parameter] protected Action<string> SelectedValueChanged { get; set; }

        #endregion
    }
}
