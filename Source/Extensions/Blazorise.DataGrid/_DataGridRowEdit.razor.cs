#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

public abstract class _BaseDataGridRowEdit<TItem> : ComponentBase, IDisposable
{
    #region Members

    protected EventCallbackFactory callbackFactory = new();

    protected Validations validations;

    protected bool isInvalid;

    protected EventCallback Cancel
        => EventCallback.Factory.Create( this, ParentDataGrid.Cancel );

    protected static readonly IFluentFlex DefaultFlex = Flex.InlineFlex;

    protected static readonly IFluentGap DefaultGap = Gap.Is2;

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

    protected async Task SaveWithValidation()
    {
        if ( await validations.ValidateAll() )
        {
            await ParentDataGrid.Save();
        }
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
                .Where( column => column.IsDisplayable || column.Displayable )
                .OrderBy( column => column.DisplayOrder );
        }
    }

    [Parameter] public Dictionary<string, CellEditContext<TItem>> CellValues { get; set; }

    [Parameter] public DataGridEditMode EditMode { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    #endregion
}