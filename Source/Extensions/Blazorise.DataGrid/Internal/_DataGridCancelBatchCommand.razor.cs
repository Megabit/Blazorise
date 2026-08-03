#region Using directives
using System;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports data grid cancel batch command behavior in DataGrid components.
/// </summary>
public partial class _DataGridCancelBatchCommand<TItem> : ComponentBase, IDisposable
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <summary>
    /// Releases resources held by the data grid cancel batch command.
    /// </summary>
    public void Dispose()
    {
        LocalizerService.LocalizationChanged -= OnLocalizationChanged;
    }

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Supplies localized text for the cancel-batch command.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Provides translated labels for the cancel-batch action.
    /// </summary>
    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// An event raised when the batch editing is canceled.
    /// </summary>
    [Parameter] public EventCallback CancelBatch { get; set; }
}