#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.DataGrid;

public partial class _DataGridColumnDropZone<TItem> : ComponentBase, IDisposable
{
    #region Methods

    public _DataGridColumnDropZone()
    {
        classBuilder = new( BuildClasses );
        styleBuilder = new( BuildStyles );
    }

    private Task OnDrop( DragEventArgs e )
    {
        if ( ParentDataGrid.columnDragStarted is not null )
        {
            return ColumnAdded.InvokeAsync( ParentDataGrid.columnDragStarted );
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

    #region Builders

    private ClassBuilder classBuilder;
    private StyleBuilder styleBuilder;

    private string classValue;
    private string styleValue;

    protected string ClassNames
        => classBuilder.Class;

    protected string StyleNames
        => styleBuilder.Styles;

    private void DirtyClasses()
    {
        classBuilder?.Dirty();
    }

    private void DirtyStyles()
    {
        styleBuilder?.Dirty();
    }

    private void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-datagrid-drop-zone" );

        if ( !string.IsNullOrWhiteSpace( Class ) )
        {
            builder.Append( Class );
        }
    }

    private void BuildStyles( StyleBuilder builder )
    {
        if ( !string.IsNullOrWhiteSpace( Style ) )
        {
            builder.Append( Style.Trim().TrimEnd( ';' ) );
        }
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

    /// <summary>
    /// Additional CSS class for the drop zone element.
    /// </summary>
    [Parameter]
    public string Class
    {
        get => classValue;
        set
        {
            if ( classValue.IsEqual( value ) )
                return;

            classValue = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Additional styles for the drop zone element.
    /// </summary>
    [Parameter]
    public string Style
    {
        get => styleValue;
        set
        {
            if ( styleValue.IsEqual( value ) )
                return;

            styleValue = value;

            DirtyStyles();
        }
    }

    #endregion
}