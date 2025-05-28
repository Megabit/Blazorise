#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.DataGrid;

public partial class _DataGridColumnDropZone<TItem> : ComponentBase, IDisposable
{
    #region Methods

    private Task OnDrop( DragEventArgs e )
    {
        if ( ParentDataGrid.columnDragged is not null )
        {
            return ColumnAdded.InvokeAsync( ParentDataGrid.columnDragged );
        }

        return Task.CompletedTask;
    }

    private Task RemoveColumn( DataGridColumn<TItem> column )
    {
        return ColumnRemoved.InvokeAsync( column );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        LocalizerService.LocalizationChanged -= OnLocalizationChanged;
    }

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizer"/> for <see cref="DataGrid{TItem}"/> />.
    /// </summary>
    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    [Parameter] public EventCallback<DataGridColumn<TItem>> ColumnRemoved { get; set; }

    [Parameter] public EventCallback<DataGridColumn<TItem>> ColumnAdded { get; set; }

    [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

    #endregion
}