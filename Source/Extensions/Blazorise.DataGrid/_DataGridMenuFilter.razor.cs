#region Using directives
using System;
using Blazorise.Extensions;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

public partial class _DataGridMenuFilter<TItem> : ComponentBase, IDisposable
{
    #region Methods

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

    private object GetFilterValue1()
    {
        return ( Column.Filter.SearchValue as object[] )?[0];
    }

    private object GetFilterValue2()
    {
        return ( Column.Filter.SearchValue as object[] )?[1];
    }

    private void SetFilterValue1( object value1 )
    {
        if ( Column.Filter.SearchValue is not object[] )
        {
            Column.Filter.SearchValue = new object[2];
        }

        ( Column.Filter.SearchValue as object[] )[0] = value1;
    }

    private void SetFilterValue2( object value2 )
    {
        if ( Column.Filter.SearchValue is not object[] )
        {
            Column.Filter.SearchValue = new object[2];
        }

        ( Column.Filter.SearchValue as object[] )[1] = value2;
    }

    protected bool IsFiltering()
    {
        if ( !string.IsNullOrEmpty( Column.Filter.SearchValue?.ToString() ) )
            return true;

        var rangeSearchValues = Column.Filter.SearchValue as object[];

        if ( rangeSearchValues.IsNullOrEmpty() || rangeSearchValues.Length < 2 )
        {
            return false;
        }

        return !string.IsNullOrEmpty( rangeSearchValues[0]?.ToString() )
            || !string.IsNullOrEmpty( rangeSearchValues[1]?.ToString() );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizer"/> for <see cref="DataGrid{TItem}"/> />.
    /// </summary>
    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    [Parameter] public DataGridColumn<TItem> Column { get; set; }

    #endregion
}