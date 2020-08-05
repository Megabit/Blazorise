#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridModal<TItem> : ComponentBase
    {
        #region Members

        protected EventCallbackFactory callbackFactory = new EventCallbackFactory();

        protected Validations validations;

        protected bool popupVisible;

        #endregion

        #region Methods

        protected void ValidationsStatusChanged( ValidationsStatusChangedEventArgs args )
        {
            StateHasChanged();
        }

        protected void SaveWithValidation()
        {
            var isValid = validations.ValidateAll();

            if ( isValid )
                Save.InvokeAsync( this );
        }

        #endregion

        #region Properties

        [Parameter] public TItem EditItem { get; set; }

        [Parameter] public RenderFragment<PopupTitleContext<TItem>> TitleTemplate { get; set; }

        [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

        [Parameter] public IReadOnlyDictionary<string, CellEditContext> EditItemCellValues { get; set; }

        [Parameter]
        public bool PopupVisible
        {
            get => popupVisible;
            set
            {
                if ( !popupVisible && value )
                {
                    validations?.ClearAll();
                }

                popupVisible = value;
            }
        }

        [Parameter] public ModalSize PopupSize { get; set; }

        [Parameter] public DataGridEditState EditState { get; set; }

        [Parameter] public EventCallback Save { get; set; }

        [Parameter] public EventCallback Cancel { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        #endregion
    }
}
