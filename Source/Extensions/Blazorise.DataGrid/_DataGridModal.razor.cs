#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridModal<TItem> : ComponentBase, IDisposable
    {
        #region Members

        protected EventCallbackFactory callbackFactory = new EventCallbackFactory();

        protected Validations validations;

        protected bool popupVisible;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            LocalizerService.LocalizationChanged += OnLocalizationChanged;

            base.OnInitialized();
        }

        public void Dispose()
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }

        private async void OnLocalizationChanged( object sender, EventArgs e )
        {
            await InvokeAsync( StateHasChanged );
        }

        protected void ValidationsStatusChanged( ValidationsStatusChangedEventArgs args )
        {
            InvokeAsync( StateHasChanged );
        }

        protected void SaveWithValidation()
        {
            var isValid = validations.ValidateAll();

            if ( isValid )
                Save.InvokeAsync( this );
        }

        #endregion

        #region Properties

        [Inject] protected ITextLocalizerService LocalizerService { get; set; }

        [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

        [Parameter] public TItem EditItem { get; set; }

        [Parameter] public RenderFragment<PopupTitleContext<TItem>> TitleTemplate { get; set; }

        [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

        protected IEnumerable<DataGridColumn<TItem>> OrderedEditableColumns
        {
            get
            {
                return Columns
                    .Where( column => !column.ExcludeFromEdit && column.CellValueIsEditable )
                    .OrderBy( column => column.EditOrder ?? column.DisplayOrder );
            }
        }

        [Parameter] public IReadOnlyDictionary<string, CellEditContext<TItem>> EditItemCellValues { get; set; }

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
