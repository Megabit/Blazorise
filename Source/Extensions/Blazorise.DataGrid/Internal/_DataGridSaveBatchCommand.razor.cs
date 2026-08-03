#region Using directives
using System;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports data grid save batch command behavior in DataGrid components.
/// </summary>
public partial class _DataGridSaveBatchCommand<TItem> : ComponentBase, IDisposable
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <summary>
    /// Releases resources held by the data grid save batch command.
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
    /// Supplies localized text for the save-batch command.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Provides translated labels for the save-batch action.
    /// </summary>
    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// Handles confirmation of the pending batch changes.
    /// </summary>
    [Parameter] public EventCallback SaveBatch { get; set; }
}