#region Using directives
using System;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Renders the compact default loading indicator for DataGrid.
/// </summary>
public partial class _DataGridLoadingIndicator<TItem> : ComponentBase, IDisposable
{
    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChangedHandler;

        base.OnInitialized();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        LocalizerService.LocalizationChanged -= OnLocalizationChangedHandler;
    }

    private async void OnLocalizationChangedHandler( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides notifications when the current localization changes.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Supplies translated loading labels.
    /// </summary>
    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    #endregion
}