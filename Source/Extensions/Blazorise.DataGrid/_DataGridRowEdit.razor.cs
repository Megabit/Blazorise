﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRowEdit<TItem> : ComponentBase, IDisposable
    {
        #region Members    

        protected EventCallbackFactory callbackFactory = new EventCallbackFactory();

        protected Validations validations;

        protected bool isInvalid;

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
            isInvalid = args.Status == ValidationStatus.Error;

            InvokeAsync( StateHasChanged );
        }

        protected void SaveWithValidation()
        {
            validations.ValidateAll();

            if ( !isInvalid )
                Save.InvokeAsync( this );
        }

        #endregion

        #region Properties

        [Inject] protected ITextLocalizerService LocalizerService { get; set; }

        [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

        [Parameter] public TItem Item { get; set; }

        [Parameter] public TItem ValidationItem { get; set; }

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

        protected IEnumerable<DataGridColumn<TItem>> DisplayableColumns
        {
            get
            {
                return Columns
                    .Where( column => column.IsCommandColumn || column.IsMultiSelectColumn || column.Displayable )
                    .OrderBy( column => column.DisplayOrder );
            }
        }

        [Parameter] public Dictionary<string, CellEditContext<TItem>> CellValues { get; set; }

        [Parameter] public DataGridEditMode EditMode { get; set; }

        [Parameter] public EventCallback Save { get; set; }

        [Parameter] public EventCallback Cancel { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
        /// </summary>
        [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public string Width { get; set; }

        #endregion
    }
}
