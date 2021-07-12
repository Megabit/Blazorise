#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;

#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridModal<TItem> : ComponentBase, IDisposable
    {
        #region Members

        protected EventCallbackFactory callbackFactory = new();

        protected Validations validations;

        protected bool popupVisible;

        protected EventCallback Cancel
            => EventCallback.Factory.Create( this, ParentDataGrid.Cancel );

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

        protected Task SaveWithValidation()
        {
            var isValid = validations.ValidateAll();

            if ( isValid )
                return ParentDataGrid.Save();

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        [Inject] protected ITextLocalizerService LocalizerService { get; set; }

        [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

        [Parameter] public TItem EditItem { get; set; }

        [Parameter] public TItem ValidationItem { get; set; }

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

        /// <summary>
        /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
        /// </summary>
        [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

        #endregion
    }
}